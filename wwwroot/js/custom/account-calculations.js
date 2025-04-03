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
const handleQtyChange = (hdForm, dtForm, decimals, visible) => {
    const billQty = dtForm.getValues()?.billQTY;
    const unitPrice = dtForm.getValues()?.unitPrice;
    const exchangeRate = hdForm.getValues()?.exhRate;

    if (billQty && unitPrice) {
        const totAmt = calculateMultiplierAmount(billQty, unitPrice, decimals?.amtDec);
        dtForm.setValue("totAmt", totAmt);
        dtForm.trigger("totAmt");

        if (exchangeRate) {
            handleTotalAmountChange(hdForm, dtForm, decimals, visible);
        }
    }
};

// Handle Total Amount Change
const handleTotalAmountChange = (hdForm, dtForm, decimals, visible) => {
    const totAmt = dtForm.getValues()?.totAmt;
    const exchangeRate = hdForm.getValues()?.exhRate;

    if (exchangeRate) {
        const totLocalAmt = calculateMultiplierAmount(totAmt, exchangeRate, decimals?.locAmtDec);
        dtForm.setValue("totLocalAmt", totLocalAmt);
        dtForm.trigger("totLocalAmt");

        handleTotalCityAmountChange(hdForm, dtForm, decimals, visible);

        if (totAmt) {
            handleGstPercentageChange(hdForm, dtForm, decimals, visible);
        }
    }
};

// Handle GST Percentage Change
const handleGstPercentageChange = (hdForm, dtForm, decimals, visible) => {
    const totAmt = dtForm.getValues()?.totAmt;
    const gstRate = dtForm.getValues()?.gstPercentage;

    if (totAmt) {
        const gstAmt = calculatePercentageAmount(totAmt, gstRate, decimals?.amtDec);
        dtForm.setValue("gstAmt", gstAmt);
        dtForm.trigger("gstAmt");

        const exchangeRate = hdForm.getValues()?.exhRate;
        if (exchangeRate) {
            const gstLocalAmt = calculateMultiplierAmount(gstAmt, exchangeRate, decimals?.locAmtDec);
            dtForm.setValue("gstLocalAmt", gstLocalAmt);
            dtForm.trigger("gstLocalAmt");

            handleGstCityPercentageChange(hdForm, dtForm, decimals, visible);
        }
    }
};

// Handle Total City Amount Change
const handleTotalCityAmountChange = (hdForm, dtForm, decimals, visible) => {
    const totAmt = dtForm.getValues()?.totAmt;
    const exchangeRate = hdForm.getValues()?.exhRate;
    const cityExchangeRate = hdForm.getValues()?.ctyExhRate;
    let totCtyAmt = 0;

    if (visible?.m_CtyCurr) {
        totCtyAmt = calculateMultiplierAmount(totAmt, cityExchangeRate, decimals?.locAmtDec);
    } else {
        totCtyAmt = calculateMultiplierAmount(totAmt, exchangeRate, decimals?.locAmtDec);
    }

    dtForm.setValue("totCtyAmt", totCtyAmt);
    dtForm.trigger("totCtyAmt");
};

// Handle GST City Percentage Change
const handleGstCityPercentageChange = (hdForm, dtForm, decimals, visible) => {
    const gstAmt = dtForm.getValues()?.gstAmt;
    const exchangeRate = hdForm.getValues()?.exhRate;
    const cityExchangeRate = hdForm.getValues()?.ctyExhRate;
    let gstCtyAmt = 0;

    if (visible?.m_CtyCurr) {
        gstCtyAmt = calculateMultiplierAmount(gstAmt, cityExchangeRate, decimals?.locAmtDec);
    } else {
        gstCtyAmt = calculateMultiplierAmount(gstAmt, exchangeRate, decimals?.locAmtDec);
    }

    dtForm.setValue("gstCtyAmt", gstCtyAmt);
    dtForm.trigger("gstCtyAmt");
};