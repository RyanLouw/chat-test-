window.AppLoader = {
    show: function (text) {
        const loader = document.getElementById("globalLoader");
        if (!loader) return;

        const textElement = loader.querySelector(".global-loader-text");

        if (textElement) {
            textElement.textContent = text || "Loading...";
        }

        loader.classList.remove("d-none");
    },

    hide: function () {
        const loader = document.getElementById("globalLoader");
        if (!loader) return;

        loader.classList.add("d-none");
    }
};

document.addEventListener("DOMContentLoaded", function () {
    document.querySelectorAll(".js-show-loader").forEach(function (element) {
        element.addEventListener("click", function () {
            const text = element.dataset.loaderText || "Loading...";
            AppLoader.show(text);
        });
    });

    document.querySelectorAll("form.js-loader-form").forEach(function (form) {
        form.addEventListener("submit", function () {
            const text = form.dataset.loaderText || "Loading...";
            AppLoader.show(text);
        });
    });
});

window.addEventListener("pageshow", function () {
    AppLoader.hide();
});