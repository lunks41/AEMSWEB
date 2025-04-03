function calculateInvoice(form, data, invoice, decimals) {
   
    var ctyExhRate = form.getAttribute("data-ctyExhRate");
    var exhRate = form.getAttribute("data-exhRate");

    var newInvoice = [];

    data.forEach(function (inv) {
        newInvoice.push({
            ...inv,
            gstCtyAmt: Number((inv.gstAmt * ctyExhRate).toFixed(decimals.amtDec)),
            gstLocalAmt: Number((inv.gstAmt * exhRate).toFixed(decimals.locAmtDec)),
            totLocalAmt: Number((inv.totAmt * exhRate).toFixed(decimals.locAmtDec)),
            totCtyAmt: Number((inv.totAmt * ctyExhRate).toFixed(decimals.ctyAmtDec))
        });
    });

    var totals = getInvoiceDetailsSum(newInvoice, decimals);

    document.getElementById("gstLocalAmt").value = totals.gstLocalAmt;
    document.getElementById("gstCtyAmt").value = totals.gstCtyAmt;
    document.getElementById("totLocalAmt").value = totals.totLocalAmt;
    document.getElementById("totCtyAmt").value = totals.totCtyAmt;
    document.getElementById("totAmt").value = totals.totAmt;
    document.getElementById("gstAmt").value = totals.gstAmt;

    document.getElementById("totAmtAftGst").value = Number((totals.totAmt + totals.gstAmt).toFixed(decimals.amtDec));
    document.getElementById("totLocalAmtAftGst").value = Number((totals.gstLocalAmt + totals.totLocalAmt).toFixed(decimals.locAmtDec));
    document.getElementById("totCtyAmtAftGst").value = Number((totals.gstCtyAmt + totals.totCtyAmt).toFixed(decimals.ctyAmtDec));
}

function getInvoiceDetailsSum(invoices, decimals) {
   
    var gstLocalAmt = 0;
    var gstCtyAmt = 0;
    var totLocalAmt = 0;
    var totCtyAmt = 0;
    var totAmt = 0;
    var gstAmt = 0;

    invoices.forEach(function (item) {
        gstLocalAmt += item.gstLocalAmt;
        gstCtyAmt += item.gstCtyAmt;
        totLocalAmt += item.totLocalAmt;
        totCtyAmt += item.totCtyAmt;
        totAmt += item.totAmt;
        gstAmt += item.gstAmt;
    });

    return {
        gstLocalAmt: Number(gstLocalAmt.toFixed(decimals.locAmtDec)),
        gstCtyAmt: Number(gstCtyAmt.toFixed(decimals.ctyAmtDec)),
        totLocalAmt: Number(totLocalAmt.toFixed(decimals.locAmtDec)),
        totCtyAmt: Number(totCtyAmt.toFixed(decimals.ctyAmtDec)),
        totAmt: Number(totAmt.toFixed(decimals.amtDec)),
        gstAmt: Number(gstAmt.toFixed(decimals.amtDec))
    };
}