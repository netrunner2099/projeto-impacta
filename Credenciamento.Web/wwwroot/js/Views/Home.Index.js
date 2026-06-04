$(document).ready(function () {
    // Inicializar carrossel com intervalo de 5 segundos
    var carousel = new bootstrap.Carousel(document.getElementById('heroCarousel'), {
        interval: 5000,
        ride: 'carousel',
        pause: 'hover'
    });

    // Smooth scroll para seção de eventos
    $('a[href="#eventos"]').click(function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: $('#eventos').offset().top - 100
        }, 800);
    });

    // Comprar ingresso
    $('.btn-buy').click(function () {
        var eventId = $(this).data('id');
        window.location.href = '/Store/Index/' + eventId;
    });

    // Contador animado
    const counters = document.querySelectorAll('.counter');
    const speed = 200;

    counters.forEach(counter => {
        const updateCount = () => {
            const target = +counter.getAttribute('data-target');
            const count = +counter.innerText;
            const inc = target / speed;

            if (count < target) {
                counter.innerText = Math.ceil(count + inc);
                setTimeout(updateCount, 1);
            } else {
                counter.innerText = target;
            }
        };

        const observer = new IntersectionObserver((entries) => {
            if (entries[0].isIntersecting) {
                updateCount();
                observer.disconnect();
            }
        });

        observer.observe(counter);
    });

    // Animação de entrada dos cards
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const cardObserver = new IntersectionObserver(function (entries) {
        entries.forEach((entry, index) => {
            if (entry.isIntersecting) {
                setTimeout(() => {
                    entry.target.style.opacity = '1';
                    entry.target.style.transform = 'translateY(0)';
                }, index * 100);
            }
        });
    }, observerOptions);

    document.querySelectorAll('.event-card').forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(30px)';
        card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        cardObserver.observe(card);
    });
});