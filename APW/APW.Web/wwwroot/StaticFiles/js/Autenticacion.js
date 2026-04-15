document.addEventListener("DOMContentLoaded", () => {
    const confirmButtonColor =
        getComputedStyle(document.documentElement).getPropertyValue("--accent").trim() || "#1e5aff";

    const swalMessages = document.querySelectorAll("[data-swal]");

    swalMessages.forEach((box) => {
        const type = box.getAttribute("data-type") || "info";
        const title = box.getAttribute("data-title") || "Mensaje";
        const text = box.getAttribute("data-text") || "";

        Swal.fire({
            icon: type,
            title,
            text,
            confirmButtonColor
        });
    });

    const forms = document.querySelectorAll(".js-auth-form");

    forms.forEach((form) => {
        const inputs = form.querySelectorAll(".input[required]");

        const showFieldError = (input, message) => {
            const field = input.closest(".field");
            const error = field?.querySelector(".error");

            input.classList.add("is-invalid");

            if (error) {
                error.textContent = message;
                error.style.display = "block";
            }
        };

        const clearFieldError = (input) => {
            const field = input.closest(".field");
            const error = field?.querySelector(".error");

            input.classList.remove("is-invalid");

            if (error) {
                error.textContent = "";
                error.style.display = "none";
            }
        };

        const validateInput = (input) => {
            const label = input.dataset.label || input.name || "Campo";
            const value = input.value.trim();

            if (!value) {
                showFieldError(input, `${label} es obligatorio.`);
                return `${label} es obligatorio.`;
            }

            if (input.type === "email") {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
                if (!emailRegex.test(value)) {
                    showFieldError(input, "Ingresá un correo válido.");
                    return "Ingresá un correo válido.";
                }
            }

            clearFieldError(input);
            return null;
        };

        inputs.forEach((input) => {
            input.addEventListener("blur", () => validateInput(input));
            input.addEventListener("input", () => {
                if (input.classList.contains("is-invalid")) {
                    validateInput(input);
                }
            });
        });

        form.addEventListener("submit", (event) => {
            const messages = [];

            inputs.forEach((input) => {
                const validationMessage = validateInput(input);
                if (validationMessage) {
                    messages.push(validationMessage);
                }
            });

            if (messages.length > 0) {
                event.preventDefault();

                const list = [...new Set(messages)]
                    .map((message) => `<li>${message}</li>`)
                    .join("");

                Swal.fire({
                    icon: "error",
                    title: "Revisá los campos",
                    html: `<ul style=\"text-align:left;margin:0;padding-left:1.2rem\">${list}</ul>`,
                    confirmButtonText: "OK",
                    confirmButtonColor
                });
            }
        });
    });
});
