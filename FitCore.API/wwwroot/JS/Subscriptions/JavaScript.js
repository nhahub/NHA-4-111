// الدالة دي بتشتغل لما اليوزر يدوس على زرار Subscribe Now
async function subscribeToPlan(userId, profileId, serviceId, duration, price, serviceName) {

    // 1. بنجمع الداتا اللي هنبعتها للـ API بتاعك (CreateSubscriptionDto)
    const requestData = {
        userId: userId,
        memberProfileId: profileId,
        gymServiceId: serviceId,
        durationInDays: duration,
        price: price,
        serviceName: serviceName
    };

    try {
        // 2. بنكلم الـ API اللي إنت عملتها (CreateSubscriptionWithInvoiceAsync)
        const response = await fetch('/api/Subscription/create-with-invoice', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(requestData)
        });

        if (response.ok) {
            // لو الباك إند رجع 200 OK
            alert(`تم الاشتراك بنجاح في ${serviceName} وتكوين الفاتورة!`);

            // الخطوة الجاية: ننقل اليوزر لصفحة الفاتورة والدفع اللي هنعملها بعد دي
            // window.location.href = '/html/Subscriptions/Invoice.html';
        } else {
            alert("حدث خطأ أثناء إنشاء الاشتراك. تأكد من صحة البيانات.");
        }
    } catch (error) {
        console.error("Network Error:", error);
        alert("لا يمكن الاتصال بالخادم حالياً. تأكد أن السيرفر يعمل.");
    }
}