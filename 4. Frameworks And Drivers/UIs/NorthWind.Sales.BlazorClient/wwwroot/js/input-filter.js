window.northWindInputFilter = {
    setValue: (input, value) => {
        if (input instanceof HTMLInputElement) {
            input.value = value ?? "";
        }
    }
};

document.addEventListener("beforeinput", event => {
    const input = event.target;
    if (!(input instanceof HTMLInputElement) || event.inputType.startsWith("delete")) {
        return;
    }

    const insertedText = event.data ?? event.dataTransfer?.getData("text");
    if (insertedText == null || insertedText === "") {
        return;
    }

    const mode = input.dataset.filterMode;
    const filter = input.dataset.filter;
    const permitted = character => {
        const isLetter = /\p{L}/u.test(character);
        const isDigit = /\p{N}/u.test(character);
        const isAlphanumeric = isLetter || isDigit;

        if (mode === "Address") {
            return isAlphanumeric || character === " ";
        }
        if (mode === "LettersAndSpaces") {
            return isLetter || character === " ";
        }
        if (mode === "LettersDigitsAndSpaces") {
            return isAlphanumeric || character === " ";
        }
        if (mode === "DigitsOnly") {
            return isDigit;
        }
        if (mode === "customer-search") {
            return isAlphanumeric || ((filter === "Nombre" || filter === "Todas") && character === " ");
        }
        if (mode === "product-search") {
            if (filter === "ID" || filter === "Stock" || filter === "Precio") {
                return isDigit;
            }
            return isAlphanumeric || character === " ";
        }
        if (mode === "invoice-search") {
            if (filter === "Número de factura" || filter === "Fecha") {
                return isDigit;
            }
            if (filter === "Ciudad") {
                return isLetter || character === " ";
            }
            return isAlphanumeric || character === " ";
        }
        return true;
    };

    if (![...insertedText].every(permitted)) {
        event.preventDefault();
    }
});
