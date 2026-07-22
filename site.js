(() => {
    const canvas = document.querySelector('#ambient-canvas');
    const reducedMotion = window.matchMedia('(prefers-reduced-motion: reduce)').matches;

    if (canvas && !reducedMotion) {
        const context = canvas.getContext('2d');
        const pointer = { x: window.innerWidth / 2, y: window.innerHeight / 2 };
        let particles = [];

        function resize() {
            canvas.width = window.innerWidth * window.devicePixelRatio;
            canvas.height = window.innerHeight * window.devicePixelRatio;
            canvas.style.width = `${window.innerWidth}px`;
            canvas.style.height = `${window.innerHeight}px`;
            context.setTransform(window.devicePixelRatio, 0, 0, window.devicePixelRatio, 0, 0);
            particles = Array.from({ length: Math.min(52, Math.floor(window.innerWidth / 24)) }, () => ({
                x: Math.random() * window.innerWidth,
                y: Math.random() * window.innerHeight,
                size: Math.random() * 1.6 + .4,
                speed: Math.random() * .22 + .04,
                drift: Math.random() * .5 + .15
            }));
        }

        function draw() {
            context.clearRect(0, 0, window.innerWidth, window.innerHeight);
            particles.forEach((particle, index) => {
                particle.y -= particle.speed;
                particle.x += Math.sin(Date.now() / 1800 + index) * particle.drift;
                if (particle.y < -10) { particle.y = window.innerHeight + 10; particle.x = Math.random() * window.innerWidth; }
                const distance = Math.hypot(pointer.x - particle.x, pointer.y - particle.y);
                context.fillStyle = distance < 150 ? 'rgba(111, 231, 255, .8)' : 'rgba(119, 125, 255, .42)';
                context.beginPath(); context.arc(particle.x, particle.y, particle.size, 0, Math.PI * 2); context.fill();
            });
            requestAnimationFrame(draw);
        }

        window.addEventListener('resize', resize);
        window.addEventListener('pointermove', event => { pointer.x = event.clientX; pointer.y = event.clientY; });
        resize(); draw();
    }

    const observer = new IntersectionObserver(entries => entries.forEach(entry => {
        if (entry.isIntersecting) { entry.target.classList.add('is-visible'); observer.unobserve(entry.target); }
    }), { threshold: .14 });
    document.querySelectorAll('.reveal').forEach(element => observer.observe(element));
})();
