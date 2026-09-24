/* ============================================================
   CineMagic — interactions (vanilla JS, no dependencies)
   ============================================================ */
document.addEventListener("DOMContentLoaded", () => {
    initNavbar();
    initMobileNav();
    initReveal();
    initTilt();
    initMagnetic();
    initRipple();
    initSeatMap();
    initCheckout();
    initConfetti();
});

/* ---------- Sticky navbar gets a shadow on scroll ---------- */
function initNavbar() {
    const nav = document.getElementById("navbar");
    if (!nav) return;
    const onScroll = () => nav.classList.toggle("scrolled", window.scrollY > 24);
    window.addEventListener("scroll", onScroll, { passive: true });
    onScroll();
}

/* ---------- Mobile hamburger menu ---------- */
function initMobileNav() {
    const toggle = document.getElementById("navToggle");
    const links = document.getElementById("navLinks");
    if (!toggle || !links) return;

    toggle.addEventListener("click", () => {
        const open = links.classList.toggle("open");
        toggle.classList.toggle("open", open);
        toggle.setAttribute("aria-expanded", open ? "true" : "false");
        toggle.setAttribute("aria-label", open ? "Close menu" : "Open menu");
    });

    // Close the menu when a link is tapped.
    links.addEventListener("click", e => {
        if (e.target.closest("a")) {
            links.classList.remove("open");
            toggle.classList.remove("open");
            toggle.setAttribute("aria-expanded", "false");
        }
    });
}

/* ---------- Reveal-on-scroll ---------- */
function initReveal() {
    const els = document.querySelectorAll(".reveal:not(.visible)");
    if (!("IntersectionObserver" in window) || els.length === 0) {
        els.forEach(el => el.classList.add("visible"));
        return;
    }
    const io = new IntersectionObserver(entries => {
        entries.forEach(e => {
            if (e.isIntersecting) {
                e.target.classList.add("visible");
                io.unobserve(e.target);
            }
        });
    }, { threshold: 0.12 });
    els.forEach(el => io.observe(el));
}

/* ---------- 3D tilt on [data-tilt] cards ---------- */
function initTilt() {
    const fine = window.matchMedia("(pointer: fine)").matches;
    document.querySelectorAll("[data-tilt]").forEach(card => {
        if (!fine) return; // skip tilt on touch devices
        card.addEventListener("mousemove", e => {
            const r = card.getBoundingClientRect();
            const x = (e.clientX - r.left) / r.width - 0.5;
            const y = (e.clientY - r.top) / r.height - 0.5;
            card.style.transform = `perspective(900px) rotateY(${x * 12}deg) rotateX(${-y * 12}deg) translateY(-6px)`;
        });
        card.addEventListener("mouseleave", () => {
            card.style.transition = "transform 0.5s cubic-bezier(0.22,1,0.36,1)";
            card.style.transform = "perspective(900px) rotateY(0) rotateX(0)";
            setTimeout(() => (card.style.transition = ""), 500);
        });
    });
}

/* ---------- Magnetic pull on primary CTAs ---------- */
function initMagnetic() {
    const fine = window.matchMedia("(pointer: fine)").matches;
    if (!fine) return;
    document.querySelectorAll(".magnetic").forEach(btn => {
        btn.addEventListener("mousemove", e => {
            const r = btn.getBoundingClientRect();
            const x = e.clientX - r.left - r.width / 2;
            const y = e.clientY - r.top - r.height / 2;
            btn.style.transform = `translate(${x * 0.18}px, ${y * 0.22}px)`;
        });
        btn.addEventListener("mouseleave", () => (btn.style.transform = ""));
    });
}

/* ---------- Ripple on click for .ripple elements ---------- */
function initRipple() {
    document.addEventListener("click", e => {
        const target = e.target.closest(".ripple");
        if (!target || target.disabled) return;
        const rect = target.getBoundingClientRect();
        const size = Math.max(rect.width, rect.height);
        const ink = document.createElement("span");
        ink.className = "ripple-ink";
        ink.style.width = ink.style.height = size + "px";
        ink.style.left = e.clientX - rect.left - size / 2 + "px";
        ink.style.top = e.clientY - rect.top - size / 2 + "px";
        target.appendChild(ink);
        setTimeout(() => ink.remove(), 700);
    });
}

/* ---------- Interactive seat map ---------- */
function initSeatMap() {
    const map = document.getElementById("seatMap");
    if (!map) return;

    const maxSeats = (window.SEAT_CONFIG && window.SEAT_CONFIG.maxSeats) || 8;
    const selected = new Map(); // label -> { price, category }

    const seatsInput = document.getElementById("seatsInput");
    const continueBtn = document.getElementById("continueBtn");
    const summarySeats = document.getElementById("summarySeats");
    const seatCount = document.getElementById("seatCount");
    const totalPrice = document.getElementById("totalPrice");

    const fmt = n => "₹" + n.toLocaleString("en-IN");

    function render() {
        const labels = [...selected.keys()].sort();
        seatsInput.value = labels.join(",");
        seatCount.textContent = labels.length;
        const total = [...selected.values()].reduce((s, v) => s + v.price, 0);
        totalPrice.textContent = fmt(total);
        continueBtn.disabled = labels.length === 0;

        if (labels.length === 0) {
            summarySeats.innerHTML = '<p class="muted">No seats selected yet.<br />Tap seats on the map 👆</p>';
            return;
        }
        summarySeats.innerHTML = '<div class="seat-chips">' +
            labels.map(l => `<span class="seat-chip">${l}</span>`).join("") +
            "</div>";
    }

    map.addEventListener("click", e => {
        const seat = e.target.closest(".seat");
        if (!seat || seat.classList.contains("sold")) return;
        const label = seat.dataset.seat;

        if (selected.has(label)) {
            selected.delete(label);
            seat.classList.remove("selected");
        } else {
            if (selected.size >= maxSeats) {
                seat.classList.add("shake");
                setTimeout(() => seat.classList.remove("shake"), 450);
                return;
            }
            selected.set(label, { price: Number(seat.dataset.price), category: seat.dataset.category });
            seat.classList.add("selected");
        }
        render();
    });

    render();
}

/* ---------- Checkout: Pay button loading state ---------- */
function initCheckout() {
    const form = document.getElementById("checkoutForm");
    const payBtn = document.getElementById("payBtn");
    if (!form || !payBtn) return;

    form.addEventListener("submit", e => {
        if (payBtn.classList.contains("loading")) { e.preventDefault(); return; }
        // Let the browser run its HTML5 validation first.
        if (!form.checkValidity()) return;
        e.preventDefault();
        payBtn.classList.add("loading");
        payBtn.querySelector(".pay-label").textContent = "Processing payment…";
        // Simulated gateway delay, then submit for real.
        setTimeout(() => form.submit(), 1400);
    });
}

/* ---------- Confetti on the confirmation page ---------- */
function initConfetti() {
    const canvas = document.getElementById("confettiCanvas");
    if (!canvas) return;

    const ctx = canvas.getContext("2d");
    const resize = () => { canvas.width = innerWidth; canvas.height = innerHeight; };
    resize();
    window.addEventListener("resize", resize);

    const colors = ["#f5b942", "#ffd97a", "#ef4444", "#f59e0b", "#ffffff", "#22d3ee"];
    const pieces = Array.from({ length: 160 }, () => ({
        x: Math.random() * canvas.width,
        y: Math.random() * -canvas.height - 20,
        w: 6 + Math.random() * 7,
        h: 8 + Math.random() * 8,
        color: colors[Math.floor(Math.random() * colors.length)],
        vy: 2.2 + Math.random() * 3.2,
        vx: -1.6 + Math.random() * 3.2,
        rot: Math.random() * Math.PI * 2,
        vr: -0.12 + Math.random() * 0.24
    }));

    const endAt = Date.now() + 4500;
    (function tick() {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        let alive = false;
        for (const p of pieces) {
            p.x += p.vx; p.y += p.vy; p.rot += p.vr;
            if (p.y < canvas.height + 30) alive = true;
            ctx.save();
            ctx.translate(p.x, p.y);
            ctx.rotate(p.rot);
            ctx.fillStyle = p.color;
            ctx.fillRect(-p.w / 2, -p.h / 2, p.w, p.h);
            ctx.restore();
        }
        if (alive && Date.now() < endAt + 1500) requestAnimationFrame(tick);
        else ctx.clearRect(0, 0, canvas.width, canvas.height);
    })();
}
