
const currentInvoice = {
    invoiceId: 1,
    userId: 1,
    amount: 500,
    description: "Monthly Plan"
};


document.addEventListener("DOMContentLoaded", () => {
    document.getElementById("display-invoice-id").textContent = currentInvoice.invoiceId;
    document.getElementById("display-desc").textContent = currentInvoice.description;
    document.getElementById("display-amount").textContent = currentInvoice.amount;
});


async function processPayment() {
    const methodSelect = document.getElementById("payment-method-select");
    const selectedMethod = methodSelect.options[methodSelect.selectedIndex].value;

    const paymentDto = {
        InvoiceId: currentInvoice.invoiceId,
        UserId: currentInvoice.userId,
        Amount: currentInvoice.amount,
        PaymentMethod: selectedMethod,
        TransactionReference: "TXN-" + Math.floor(Math.random() * 100000)
    };

    try {

        const response = await fetch('/api/Subscription/pay-invoice', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(paymentDto)
        });

        if (response.ok) {
            alert("تم الدفع بنجاح! الفلوس دخلت السيستم.");


            const badge = document.getElementById("invoice-status");
            badge.textContent = "Completed";
            badge.style.backgroundColor = "var(--status-green-bg)";
            badge.style.color = "var(--status-green)";

            const btn = document.querySelector(".btn-primary");
            btn.disabled = true;
            btn.textContent = "Paid Successfully";
            btn.style.backgroundColor = "var(--status-green)";

        } else {
            alert("فشلت عملية الدفع. راجع حالة الفاتورة في الداتا بيز.");
        }
    } catch (error) {
        console.error("Network Error:", error);
        alert("مشكلة في الاتصال بالسيرفر.");
    }
}