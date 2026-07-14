document.addEventListener('DOMContentLoaded', async () => {
    const token = getToken();
    if (!token) {
        window.location.href = '/html/Auth/login.html';
        return;
    }

    const API_BASE = '/api';


    const urlParams = new URLSearchParams(window.location.search);
    const invoiceId = urlParams.get('id');

    if (!invoiceId) {
        alert("Invalid Invoice ID");
        window.history.back();
        return;
    }

    try {
        const response = await fetch(`${API_BASE}/Invoices/${invoiceId}`, {
            headers: { 'Authorization': `Bearer ${token}` }
        });

        if (!response.ok) throw new Error("Invoice not found");

        const data = await response.json();
        renderInvoiceDetails(data);

        document.getElementById('loadingMessage').style.display = 'none';
        document.getElementById('invoiceReceipt').style.display = 'block';

    } catch (error) {
        console.error("Error:", error);
        document.getElementById('loadingMessage').innerHTML = `<p style="color: var(--status-red);">Failed to load invoice details.</p>`;
    }

    function renderInvoiceDetails(inv) {

        document.getElementById('invNumber').textContent = `Invoice #INV-${inv.invoiceId || inv.InvoiceId}`;
        document.getElementById('invDate').textContent = new Date(inv.issueDate || inv.IssueDate).toLocaleDateString();
        document.getElementById('invDescription').textContent = inv.description || inv.Description || "—";

        const user = getCurrentUser();
        document.getElementById('invCustomerName').textContent = user ? user.fullName : "Valued Member";


        const statusEl = document.getElementById('invStatus');
        const statusCode = inv.invoiceStatus || inv.InvoiceStatus;
        if (statusCode === 1 || statusCode === "Pending") { statusEl.textContent = "Pending"; statusEl.className = "status-pill status-pending"; }
        else if  (statusCode === 2 || statusCode === "Completed") { statusEl.textContent = "Paid"; statusEl.className = "status-pill status-completed"; }
        else { statusEl.textContent = "Cancelled"; statusEl.className = "status-pill status-cancelled"; }


        const itemsBody = document.getElementById('invItemsBody');
        const items = inv.items || inv.Items || [];
        items.forEach(item => {
            const itemTypeMap = ["Service", "Class", "Product"]; 
            const typeText = itemTypeMap[item.itemType || item.ItemType] || "Item";
            const price = (item.sellPrice || item.SellPrice || 0).toFixed(2);
            const total = (item.lineTotal || item.LineTotal || 0).toFixed(2);

            itemsBody.innerHTML += `
                <tr>
                    <td><strong>${item.itemName || item.ItemName}</strong></td>
                    <td><span class="badge" style="background:var(--bg-hover); color:var(--text-muted);">${typeText}</span></td>
                    <td>$${price}</td>
                    <td>x${item.quantity || item.Quantity}</td>
                    <td style="text-align: right; font-weight: 600;">$${total}</td>
                </tr>
            `;
        });


        const payments = inv.payments || inv.Payments || [];
        if (payments.length > 0) {
            document.getElementById('paymentsSection').style.display = 'block';
            const paymentsBody = document.getElementById('invPaymentsBody');

            payments.forEach(pay => {
                const date = new Date(pay.paymentDate || pay.PaymentDate).toLocaleDateString();
                const amount = (pay.amountPaid || pay.AmountPaid || 0).toFixed(2);
                const methodMap = ["Cash", "Card", "Stripe"]; 
                const method = methodMap[pay.paymentMethod || pay.PaymentMethod] || "Card";

                paymentsBody.innerHTML += `
                    <tr>
                        <td>${date}</td>
                        <td>${method}</td>
                        <td style="font-family: monospace; font-size:12px;">${pay.transactionReference || pay.TransactionReference || "—"}</td>
                        <td style="text-align: right; color: var(--status-green); font-weight: 600;">+$${amount}</td>
                    </tr>
                `;
            });
        }


        const sub = (inv.subTotal || inv.SubTotal || 0).toFixed(2);
        const disc = (inv.discountAmount || inv.DiscountAmount || 0).toFixed(2);
        const grand = (inv.totalAmount || inv.TotalAmount || 0).toFixed(2);

        document.getElementById('invSubtotal').textContent = `$${sub}`;
        document.getElementById('invDiscount').textContent = `-$${disc}`;
        document.getElementById('invTotal').textContent = `$${grand}`;
    }
});