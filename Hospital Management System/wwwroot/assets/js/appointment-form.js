document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('appointmentForm');
    const submitBtn = document.getElementById('submitBtn');

    if (!form || !submitBtn) return;

    form.addEventListener('submit', function (e) {
        e.preventDefault();

        // Only proceed if the button was actually clicked
        if (e.submitter !== submitBtn) return;

        handleFormSubmission(form, submitBtn);
    });

    // Separate function for better readability
    async function handleFormSubmission(form, btn) {
        const btnText = btn.querySelector('#btnText');
        const btnSpinner = btn.querySelector('#btnSpinner');

        // Set loading state
        btn.disabled = true;
        btnText.textContent = 'Processing...';
        btnSpinner.classList.remove('d-none');

        try {
            const formData = new FormData(form);
            const response = await fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: {
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                }
            });

            if (response.redirected) {
                await Swal.fire({
                    icon: 'success',
                    title: 'Success!',
                    text: 'Your appointment has been booked',
                    showConfirmButton: false,
                    timer: 2000
                });
                window.location.href = response.url;
            } else {
                const result = await response.json();
                if (!response.ok) throw new Error(result.message || 'Booking failed');
            }
        } catch (error) {
            await Swal.fire({
                icon: 'error',
                title: 'Error',
                text: error.message,
                confirmButtonText: 'Try Again'
            });
        } finally {
            // Reset button state
            btn.disabled = false;
            btnText.textContent = 'Make an Appointment';
            btnSpinner.classList.add('d-none');
        }
    }
});