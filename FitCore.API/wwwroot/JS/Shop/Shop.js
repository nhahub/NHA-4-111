// Shop page — wired to the real ShopController endpoints.
// GET  /api/Shop/products
// POST /api/Shop/cart                 body: { productID, quantity }
// GET  /api/Shop/cart
// PATCH /api/Shop/cart/{cartItemId}   body: quantity (raw number)
// DELETE /api/Shop/cart/{cartItemId}
// POST /api/Shop/checkout             body: { description }

const SHOP_ENDPOINTS = {
    products: '/api/Shop/products',
    cart: '/api/Shop/cart',
    cartItem: (id) => `/api/Shop/cart/${id}`,
    checkout: '/api/Shop/checkout',
};

const pendingQuantities = {}; // productId -> chosen quantity before "Add to Cart"

document.addEventListener('DOMContentLoaded', () => {
    loadProducts();
    wireCartDrawer();
});

// ---------------------------------------------------------------
// Products
// ---------------------------------------------------------------
async function loadProducts() {
    const grid = document.getElementById('products-grid');
    const emptyState = document.getElementById('productsEmptyState');
    if (!grid) return;

    try {
        const response = await fetch(SHOP_ENDPOINTS.products);
        if (!response.ok) throw new Error(`Failed to load products (${response.status})`);
        const products = await response.json();

        grid.innerHTML = '';

        if (!products || products.length === 0) {
            if (emptyState) emptyState.style.display = 'block';
            return;
        }
        if (emptyState) emptyState.style.display = 'none';

        products.forEach(renderProductCard);
    } catch (error) {
        console.error('Error loading products:', error);
        showBanner('shopMsgBanner', 'Could not load products. Please refresh the page.');
    }
}

function renderProductCard(product) {
    const grid = document.getElementById('products-grid');

    const id = pick(product, 'productID', 'productId', 'ProductID');
    const name = pick(product, 'name', 'Name') ?? 'Unnamed product';
    const description = pick(product, 'description', 'Description') ?? '';
    const price = Number(pick(product, 'currentSellPrice', 'CurrentSellPrice') ?? 0);
    const imageUrl = pick(product, 'imageUrl', 'ImageUrl') ?? '';

    pendingQuantities[id] = 1;

    const card = document.createElement('div');
    card.className = 'product-card';
    card.innerHTML = `
        <img class="product-img" src="/images/${escapeAttr(imageUrl.split('/').pop())}" alt="${escapeAttr(name)}" onerror="this.style.visibility='hidden'">        <h3 class="product-name">${escapeHtml(name)}</h3>
        <p class="product-desc">${escapeHtml(description)}</p>
        <span class="product-price">${formatCurrency(price)}</span>
        <div class="product-actions">
            <div class="qty-stepper">
                <button type="button" class="qty-minus" aria-label="Decrease quantity">-</button>
                <span class="qty-value">1</span>
                <button type="button" class="qty-plus" aria-label="Increase quantity">+</button>
            </div>
            <button type="button" class="add-to-cart-btn">Add to Cart</button>
        </div>
    `;

    const qtyValueEl = card.querySelector('.qty-value');
    card.querySelector('.qty-minus').addEventListener('click', () => {
        pendingQuantities[id] = Math.max(1, pendingQuantities[id] - 1);
        qtyValueEl.innerText = pendingQuantities[id];
    });
    card.querySelector('.qty-plus').addEventListener('click', () => {
        pendingQuantities[id] = pendingQuantities[id] + 1;
        qtyValueEl.innerText = pendingQuantities[id];
    });

    const addBtn = card.querySelector('.add-to-cart-btn');
    addBtn.addEventListener('click', () => addToCart(id, pendingQuantities[id], addBtn));

    grid.appendChild(card);
}

async function addToCart(productId, quantity, buttonEl) {
    if (buttonEl) {
        buttonEl.disabled = true;
        buttonEl.innerText = 'Adding…';
    }
    try {
        const response = await fetch(SHOP_ENDPOINTS.cart, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ productID: productId, quantity: quantity }),
        });

        if (!response.ok) {
            const text = await response.text();
            throw new Error(text || `Request failed (${response.status})`);
        }

        showToast('Added to cart');
        await refreshCartBadge();
    } catch (error) {
        console.error('Error adding to cart:', error);
        showToast(error.message || 'Could not add product to cart', true);
    } finally {
        if (buttonEl) {
            buttonEl.disabled = false;
            buttonEl.innerText = 'Add to Cart';
        }
    }
}

// ---------------------------------------------------------------
// Cart drawer
// ---------------------------------------------------------------
function wireCartDrawer() {
    const overlay = document.getElementById('cartOverlay');
    const drawer = document.getElementById('cartDrawer');
    const openBtn = document.getElementById('cartToggleBtn');
    const closeBtn = document.getElementById('closeCartBtn');
    const checkoutBtn = document.getElementById('checkoutBtn');

    const openDrawer = () => {
        drawer.classList.add('open');
        overlay.classList.add('open');
        loadCart();
    };
    const closeDrawer = () => {
        drawer.classList.remove('open');
        overlay.classList.remove('open');
    };

    openBtn?.addEventListener('click', openDrawer);
    closeBtn?.addEventListener('click', closeDrawer);
    overlay?.addEventListener('click', closeDrawer);
    checkoutBtn?.addEventListener('click', checkout);

    refreshCartBadge();
}

async function loadCart() {
    const container = document.getElementById('cartItems');
    const emptyState = document.getElementById('cartEmptyState');
    if (!container) return;

    try {
        const response = await fetch(SHOP_ENDPOINTS.cart);
        if (!response.ok) throw new Error(`Failed to load cart (${response.status})`);
        const items = await response.json();

        renderCartItems(items || []);
    } catch (error) {
        console.error('Error loading cart:', error);
        container.innerHTML = '';
        if (emptyState) {
            emptyState.style.display = 'block';
            emptyState.innerText = 'Could not load your cart.';
        }
    }
}

function renderCartItems(items) {
    const container = document.getElementById('cartItems');
    const emptyState = document.getElementById('cartEmptyState');

    container.innerHTML = '';

    if (!items || items.length === 0) {
        if (emptyState) {
            emptyState.style.display = 'block';
            emptyState.innerText = 'Your cart is empty.';
        }
        updateCartTotal(items);
        updateCartBadgeCount(0);
        return;
    }
    if (emptyState) emptyState.style.display = 'none';

    items.forEach(item => {
        const cartItemId = pick(item, 'cartItemID', 'cartItemId', 'CartItemID');
        const name = pick(item, 'productName', 'ProductName') ?? 'Item';
        const quantity = Number(pick(item, 'quantity', 'Quantity') ?? 1);
        const unitPrice = Number(pick(item, 'unitPrice', 'UnitPrice') ?? 0);
        const imageUrl = pick(item, 'imageUrl', 'ImageUrl') ?? '';

        const row = document.createElement('div');
        row.className = 'cart-item';
        row.innerHTML = `
<img src="/images/${escapeAttr(imageUrl.split('/').pop())}" alt="${escapeAttr(name)}" onerror="this.style.visibility='hidden'">            <div class="cart-item-info">
                <span class="cart-item-name">${escapeHtml(name)}</span>
                <span class="cart-item-price">${formatCurrency(unitPrice)} each</span>
                <div class="cart-item-controls">
                    <div class="qty-stepper">
                        <button type="button" class="qty-minus" aria-label="Decrease quantity">-</button>
                        <span class="qty-value">${quantity}</span>
                        <button type="button" class="qty-plus" aria-label="Increase quantity">+</button>
                    </div>
                    <button type="button" class="cart-item-remove">Remove</button>
                </div>
            </div>
        `;

        const qtyValueEl = row.querySelector('.qty-value');
        row.querySelector('.qty-minus').addEventListener('click', () => {
            const next = quantity - 1;
            if (next <= 0) {
                removeCartItem(cartItemId);
            } else {
                updateCartItemQuantity(cartItemId, next);
            }
        });
        row.querySelector('.qty-plus').addEventListener('click', () => {
            updateCartItemQuantity(cartItemId, quantity + 1);
        });
        row.querySelector('.cart-item-remove').addEventListener('click', () => removeCartItem(cartItemId));

        container.appendChild(row);
    });

    updateCartTotal(items);
    updateCartBadgeCount(items.reduce((sum, i) => sum + Number(pick(i, 'quantity', 'Quantity') ?? 0), 0));
}

function updateCartTotal(items) {
    const totalEl = document.getElementById('cartTotal');
    if (!totalEl) return;
    const total = (items || []).reduce((sum, i) => {
        const qty = Number(pick(i, 'quantity', 'Quantity') ?? 0);
        const price = Number(pick(i, 'unitPrice', 'UnitPrice') ?? 0);
        return sum + qty * price;
    }, 0);
    totalEl.innerText = formatCurrency(total);
}

async function updateCartItemQuantity(cartItemId, newQuantity) {
    try {
        const response = await fetch(SHOP_ENDPOINTS.cartItem(cartItemId), {
            method: 'PATCH',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newQuantity),
        });
        if (!response.ok) throw new Error(`Failed to update quantity (${response.status})`);
        await loadCart();
    } catch (error) {
        console.error('Error updating quantity:', error);
        showToast('Could not update quantity', true);
    }
}

async function removeCartItem(cartItemId) {
    try {
        const response = await fetch(SHOP_ENDPOINTS.cartItem(cartItemId), { method: 'DELETE' });
        if (!response.ok) throw new Error(`Failed to remove item (${response.status})`);
        await loadCart();
        showToast('Item removed');
    } catch (error) {
        console.error('Error removing item:', error);
        showToast('Could not remove item', true);
    }
}

async function refreshCartBadge() {
    try {
        const response = await fetch(SHOP_ENDPOINTS.cart);
        if (!response.ok) return;
        const items = await response.json();
        updateCartBadgeCount((items || []).reduce((sum, i) => sum + Number(pick(i, 'quantity', 'Quantity') ?? 0), 0));
    } catch (error) {
        console.error('Error refreshing cart badge:', error);
    }
}

function updateCartBadgeCount(count) {
    const badge = document.getElementById('cartCount');
    if (!badge) return;
    badge.innerText = count;
    badge.style.display = count > 0 ? 'flex' : 'none';
}

// ---------------------------------------------------------------
// Checkout
// ---------------------------------------------------------------
async function checkout() {
    const checkoutBtn = document.getElementById('checkoutBtn');
    const description = document.getElementById('checkoutDescription')?.value || '';

    // Capture the cart total *before* checkout clears it, so we can hand it
    // off to the Invoice/payment page (the API has no "get invoice by id"
    // endpoint yet, so this is the only way that page can know the amount).
    const totalText = document.getElementById('cartTotal')?.innerText || '';
    const totalAmount = Number(totalText.replace(/[^0-9.]/g, '')) || 0;

    checkoutBtn.disabled = true;
    checkoutBtn.innerText = 'Processing…';

    try {
        const response = await fetch(SHOP_ENDPOINTS.checkout, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ description }),
        });

        if (!response.ok) {
            const text = await response.text();
            throw new Error(text || `Checkout failed (${response.status})`);
        }

        const result = await response.json();
        const invoiceId = pick(result, 'invoiceId', 'InvoiceId');

        // Hand the invoice off to the payment page and navigate there.
        sessionStorage.setItem('fitcore_pending_invoice', JSON.stringify({
            invoiceId: invoiceId,
            amount: totalAmount,
            description: description || 'Shop order',
        }));

        window.location.href = '/html/Invoice/Payment.html';
    } catch (error) {
        console.error('Error during checkout:', error);
        showToast(error.message || 'Checkout failed', true);
        checkoutBtn.disabled = false;
        checkoutBtn.innerText = 'Checkout';
    }
}

// ---------------------------------------------------------------
// Small helpers
// ---------------------------------------------------------------
function pick(obj, ...keys) {
    for (const key of keys) {
        if (obj && obj[key] !== undefined && obj[key] !== null) return obj[key];
    }
    return undefined;
}

function formatCurrency(value) {
    const num = Number(value) || 0;
    return num.toLocaleString(undefined, { style: 'currency', currency: 'USD' });
}

function escapeHtml(str) {
    return String(str).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
}

function escapeAttr(str) {
    return String(str).replace(/"/g, '&quot;');
}

function showBanner(elId, message) {
    const banner = document.getElementById(elId);
    if (!banner) return;
    banner.innerText = message;
    banner.style.display = 'block';
}

let toastTimeout;
function showToast(message, isError = false) {
    const toast = document.getElementById('toast');
    if (!toast) return;
    clearTimeout(toastTimeout);
    toast.innerText = message;
    toast.classList.toggle('toast-error', isError);
    toast.classList.add('show');
    toastTimeout = setTimeout(() => toast.classList.remove('show'), 2500);
}
