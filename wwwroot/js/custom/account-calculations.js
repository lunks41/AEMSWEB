// Function to round a number to a specified precision
const mathRound = (amtValue, precision) => {
    const factor = Math.pow(10, precision);
    return Math.round(amtValue * factor) / factor;
};

// General function to calculate multiplier amount and precision
const calculateMultiplierAmount = (baseAmount, multiplier, precision) => {
    const total = baseAmount * multiplier;
    return mathRound(total, precision);
};

// General function to calculate addition amount and precision
const calculateAdditionAmount = (baseAmount, additionAmt, precision) => {
    const total = baseAmount + additionAmt;
    return mathRound(total, precision);
};

// General function to calculate percentage amount and precision
const calculatePercentageAmount = (baseAmount, percentage, precision) => {
    const total = (baseAmount * percentage) / 100;
    return mathRound(total, precision);
};

// General function to calculate division amount and precision
const calculateDivisionAmount = (baseAmount, divisor, precision) => {
    if (divisor === 0) return 0; // Avoid division by zero
    const total = baseAmount / divisor;
    return mathRound(total, precision);
};

// General function to calculate subtraction amount and precision
const calculateSubtractionAmount = (baseAmount, subtractAmt, precision) => {
    const total = baseAmount - subtractAmt;
    return mathRound(total, precision);
};


// Handle Quantity Change
function handleQtyChange() {
    const qty = parseFloat($("#qtyDt").val()) || 0;
    const unitPrice = parseFloat($("#unitPriceDt").val()) || 0;
    const exchangeRate = parseFloat($("#exhRateHd").val()) || 0;

    if (qty && unitPrice) {
        const totAmt = calculateMultiplierAmount(qty, unitPrice, 2);
        $("#totAmtDt").val(totAmt);
        if (exchangeRate)
            handleTotalAmountChange();
    }
}

// Handle Total Amount Change
function handleTotalAmountChange() {
    const totAmt = parseFloat($("#totAmtDt").val()) || 0;
    const gstPercent = parseFloat($("#gstPercentageDt").val()) || 0;
    const exchangeRate = parseFloat($("#exhRateHd").val()) || 0;

    const totLocalAmt = calculateMultiplierAmount(totAmt, exchangeRate, 2);
    $("totLocalAmtDt", totLocalAmt);

    // Calculate GST
    const gstAmt = calculatePercentageAmount(totAmt, gstPercent, 2);
    $("#gstAmtDt").val(gstAmt);

    // Calculate Total After GST
    const totalAfterGst = calculateAdditionAmount(totAmt, gstAmt, 2);
    $("#totAmtAftGstDt").val(totalAfterGst);
}

// Handle GST Percentage Change
function handleGstPercentageChange() {
    const totAmt = parseFloat($("#totAmtDt").val()) || 0;
    const gstPercent = parseFloat($("#gstPercentageDt").val()) || 0;

    if (totAmt) {
        const gstAmt = calculatePercentageAmount(totAmt, gstPercent, 2);
        $("gstAmtDt", gstAmt);

        const exchangeRate = parseFloat($("#exhRateHd").val()) || 0;
        if (exchangeRate) {
            const gstLocalAmt = calculateMultiplierAmount(gstAmt, exchangeRate, 2);
            $("gstLocalAmtDt", gstLocalAmt);
        }
    }
}
