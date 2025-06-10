document.addEventListener("DOMContentLoaded", function () {
    const tabs = document.querySelectorAll(".custom-tab");
    const dropdowns = document.querySelectorAll(".dropdown-content");

    tabs.forEach(tab => {
        const targetId = tab.getAttribute("data-target");

        tab.addEventListener("mouseenter", () => {
            dropdowns.forEach(d => d.classList.add("d-none"));
            if (targetId) {
                const target = document.getElementById(targetId);
                if (target) {
                    target.classList.remove("d-none");
                }
            }
        });
    });

    // Ẩn dropdown nếu di chuột ra ngoài khu vực nav + dropdown
    const header = document.querySelector("header");
    header.addEventListener("mouseleave", () => {
        dropdowns.forEach(d => d.classList.add("d-none"));
    });
});
