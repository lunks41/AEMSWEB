function getUrlParameter() {
    // Check if companyId is passed as a query parameter
    const urlParams = new URLSearchParams(window.location.search);
    const companyIdFromQuery = urlParams.get('companyId');

    // Since the route pattern is "{companyId}/...", the first segment is the companyId
    const pathSegments = window.location.pathname.split('/').filter(segment => segment);
    const companyIdFromRoute = pathSegments.length > 0 ? pathSegments[0] : null;

    // Fallback to cookies if needed (assuming you have a getCookie function)
    const companyIdFromCookies = getCookie('CompanyId');

    // Determine the final companyId value
    const finalCompanyId = companyIdFromQuery || companyIdFromRoute || companyIdFromCookies || null;

    // Optionally, store it in session storage
    sessionStorage.setItem('companyId', finalCompanyId);

    return finalCompanyId;
}

function getCookie(name) {
    const nameEQ = name + "=";
    const cookies = document.cookie.split(';');
    for (let cookie of cookies) {
        cookie = cookie.trim();
        if (cookie.indexOf(nameEQ) === 0) {
            return cookie.substring(nameEQ.length);
        }
    }
    return null;
}

function initializeKendoGrid(gridId, url, params, columns) {
    debugger;
    $("#" + gridId).kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: url,
                    dataType: "json",
                    // Map Kendo's parameters to your server's parameters
                    data: function (options) {
                        debugger;
                        return {
                            ...params, // Static params (searchString, companyId)
                            pageNumber: options.page, // Current page number
                            pageSize: options.pageSize // Current page size
                        };
                    }
                }
            },
            serverPaging: true, // Enable server-side paging
            serverFiltering: true,
            //serverSorting: true,
            pageSize: params.pageSize || 20, // Default page size
            autoSync: true,
            schema: {
                data: "data", // Field containing the data array in the response
                total: "total" // Field containing the total record count
            }
        },
        scrollable: true,
        columnMenu: {
                    filterable: false
                },
        filterable: { mode: "row" },
        sortable: true,
        navigatable: true,
        resizable: true,
        reorderable: true,
        pageable: true,
        toolbar: ["excel", "pdf", "search"],
        pdfExport: function (e) {
            const width = e.sender.wrapper.width();
            e.sender.wrapperClone.width(width);
            e.sender.wrapperClone.addClass('k-clone');
        },
        columns: columns
    });
}

//function initializeKendoGrid(gridId, url, params, columns) {
//    $("#" + gridId).kendoGrid({
//        dataSource: {
//            transport: {
//                read: {
//                    url: url,
//                    data: params,
//                    dataType: "json"
//                }
//            },
//            pageSize: 15,
//            schema: {
//                // If the server returns a raw array:
//                data: function (response) {
//                    debugger;
//                    return response; // Return the raw array
//                },
//                total: function (response) {
//                    debugger;
//                    return response.length; // Use for client-side paging
//                }
//            },
//        },
//        filterable: {
//            mode: "row"
//        },
//        pageable: {
//            refresh: true,
//            pageSizes: [10, 20, 50, 100],
//            buttonCount: 5
//        },
//        sortable: true,
//        navigatable: true,
//        resizable: true,
//        reorderable: true,
//        toolbar: ["search"],
//        columns: columns
//    });
//}

async function BindComboBox(url, dropdownId, textField, valueField) {
    try {
        const response = await $.ajax({
            url: url,
            type: "GET",
            cache: true // Enable caching for static data
        });

        const data = typeof response === "string" ? JSON.parse(response) : response;

        // Initialize ComboBox
        initComboBox(dropdownId, textField, valueField, data);
    } catch (error) {
        console.error("ComboBox error:", error);
        initComboBox(dropdownId, textField, valueField, []);
    }
}

function initComboBox(dropdownId, textField, valueField, data) {
    $("#" + dropdownId).kendoComboBox({
        dataSource: {
            data: data,
            serverFiltering: true // Enable server-side filtering
        },
        dataTextField: textField,
        dataValueField: valueField,
        filter: "contains",
        minLength: 3 // Wait for 3 characters before filtering
    }).data("kendoComboBox");
}

function BindAutoCompleteComboBoxData(url, dropdownId, textField, valueField) {
    $.ajax({
        url: url,
        async: false, // Synchronous request (not recommended for modern apps)
        type: "GET",
        cache: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            try {
                // Parse the response if it's a JSON string
                const data = typeof response === "string" ? JSON.parse(response) : response;

                // Initialize the Kendo AutoComplete
                $("#" + dropdownId).kendoAutoComplete({
                    dataTextField: textField,
                    dataValueField: valueField,
                    filter: "contains",
                    dataSource: data || [], // Use empty array if no data is available
                    select: function (e) {
                        const selectedItem = this.dataItem(e.item.index());
                        if (selectedItem) {
                            OnSelectDropdown(selectedItem, dropdownId);
                        }
                    },
                    change: function () {
                        const selectedItem = this.dataItem();
                        if (!selectedItem) {
                            const autoComplete = $("#" + dropdownId).data("kendoAutoComplete");
                            autoComplete.text("");
                            autoComplete.value("");
                        } else {
                            OnSelectDropdown(selectedItem, dropdownId);
                        }
                    }
                });

                // Adjust the width of the dropdown list
                AutoWithBindAutoCompleteComboBox(dropdownId);

                // Trigger additional logic after binding
                SelectedDropdown(dropdownId);
            } catch (error) {
                console.error("Error initializing Kendo AutoComplete:", error);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data for AutoComplete:", error);

            // Initialize the AutoComplete with an empty data source in case of failure
            $("#" + dropdownId).kendoAutoComplete({
                dataSource: [],
                placeholder: "Select...",
                filter: "contains"
            });
        }
    });
}

function AutoWithBindAutoCompleteComboBox(dropdownId) {
    const autoComplete = $("#" + dropdownId).data("kendoAutoComplete");
    if (autoComplete) {
        const listWidth = autoComplete.list.width();
        if (listWidth > 100) {
            autoComplete.list.width("auto");
        }
    }
}

function BindComboBox(url, dropdownId, textField, valueField) {
    try {
        $.ajax({
            url: url,
            async: false, // Synchronous request (not recommended for modern apps)
            type: "GET",
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                try {
                    // Parse the response if it's a JSON string
                    const data = typeof response === "string" ? JSON.parse(response) : response;

                    // Initialize the Kendo ComboBox
                    $("#" + dropdownId).kendoComboBox({
                        dataTextField: textField,
                        dataValueField: valueField,
                        placeholder: "Select...",
                        filter: "contains",
                        dataSource: data || [], // Use empty array if no data is available
                        select: function (e) {
                            const selectedItem = this.dataItem(e.item.index());
                            if (selectedItem) {
                                OnSelectDropdown(selectedItem, dropdownId);
                            }
                        },
                        change: function () {
                            const selectedItem = this.dataItem();
                            if (!selectedItem) {
                                const comboBox = $("#" + dropdownId).data("kendoComboBox");
                                comboBox.text("");
                                comboBox.value("");
                            } else {
                                OnSelectDropdown(selectedItem, dropdownId);
                            }
                        }
                    });

                    // Adjust the width of the dropdown list
                    AutoWithBindComboBox(dropdownId);

                    // Trigger additional logic after binding
                    SelectedDropdown(dropdownId);
                } catch (error) {
                    console.error("Error initializing Kendo ComboBox:", error);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data for ComboBox:", error);

                // Initialize the ComboBox with an empty data source in case of failure
                $("#" + dropdownId).kendoComboBox({
                    dataSource: [],
                    placeholder: "Select...",
                    filter: "contains"
                });
            }
        });
    } catch (e) {
    }
}

function AutoWithBindComboBox(dropdownId) {
    const comboBox = $("#" + dropdownId).data("kendoComboBox");
    if (comboBox) {
        const listWidth = comboBox.list.width();
        if (listWidth > 100) {
            comboBox.list.width("auto");
        }
    }
}

function BindMultiColumnComboBox(url, dropdownId, textField, valueField, columnsProperties, filterFields) {
    $.ajax({
        url: url,
        async: false, // Synchronous request (not recommended for modern apps)
        type: "GET",
        cache: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            try {
                // Parse the response if it's a JSON string
                const data = typeof response === "string" ? JSON.parse(response) : response;

                // Initialize the Kendo MultiColumnComboBox
                $("#" + dropdownId).kendoMultiColumnComboBox({
                    dataTextField: textField,
                    dataValueField: valueField,
                    placeholder: "Select...",
                    filter: "contains",
                    dataSource: data || [], // Use empty array if no data is available
                    columns: columnsProperties || [],
                    footerTemplate: 'Total #: instance.dataSource.total() # items found',
                    filterFields: filterFields || [],
                    autoWidth: true,
                    select: function (e) {
                        const selectedItem = this.dataItem(e.item.index());
                        if (selectedItem) {
                            OnSelectDropdown(selectedItem, dropdownId);
                        }
                    },
                    change: function () {
                        const selectedItem = this.dataItem();
                        if (!selectedItem) {
                            const multiColumnComboBox = $("#" + dropdownId).data("kendoMultiColumnComboBox");
                            multiColumnComboBox.text("");
                            multiColumnComboBox.value("");
                        } else {
                            OnSelectDropdown(selectedItem, dropdownId);
                        }
                    }
                });

                // Adjust the width of the dropdown list
                AutoWithBindMultiColumnComboBox(dropdownId);

                // Trigger additional logic after binding
                SelectedDropdown(dropdownId);
            } catch (error) {
                console.error("Error initializing Kendo MultiColumnComboBox:", error);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data for MultiColumnComboBox:", error);

            // Initialize the MultiColumnComboBox with an empty data source in case of failure
            $("#" + dropdownId).kendoMultiColumnComboBox({
                dataSource: [],
                placeholder: "Select...",
                filter: "contains"
            });
        }
    });
}

function AutoWithBindMultiColumnComboBox(dropdownId) {
    const multiColumnComboBox = $("#" + dropdownId).data("kendoMultiColumnComboBox");
    if (multiColumnComboBox) {
        const listWidth = multiColumnComboBox.list.width();
        if (listWidth > 50) {
            multiColumnComboBox.list.width("auto");
        }
    }
}

function initializeKendoDatePicker(selector, options = {}) {
    const defaults = {
        format: "dd/MM/yyyy",
        parseFormats: [
            "dd/MM/yy",
            "dd/MMM/yyyy",
            "MMM/dd/yyyy",
            "dd-MM-yyyy",
            "MM/dd/yyyy",
            "yyyy-MM-dd",
            "dd MMM yyyy"
        ],
        culture: "en-US",
        animation: false,
        dateInput: true,
        value: new Date(),
        footer: "Today - #:kendo.toString(data, 'd') #",
        weekNumber: true,
        month: {
            content: '<div class="k-link">#= data.value #</div>'
        },
        change: function (e) {
            if (!this.value()) {
                this.value(new Date());
            }
        }
    };

    // Merge user options with defaults
    const config = { ...defaults, ...options };

    // Initialize datepicker
    $(selector).each(function () {
        if (!$(this).data("kendoDatePicker")) {
            $(this).kendoDatePicker(config);
        }
    });
}

//function BindKendoWidget(url, dropdownId, textField, valueField, widgetType, options = {}) {
//    $.ajax({
//        url: url,
//        async: false,
//        type: "GET",
//        cache: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (response) {
//            // Parse the response if it's a JSON string
//            const data = typeof response === "string" ? JSON.parse(response) : response;

//            // Default configuration for the widget
//            const defaultOptions = {
//                dataTextField: textField,
//                dataValueField: valueField,
//                placeholder: "Select...",
//                filter: "contains",
//                dataSource: data || [],
//                autoWidth: true,
//                select: function (e) {
//                    const selectedItem = this.dataItem(e.item.index());
//                    if (selectedItem) {
//                        OnSelectDropdown(selectedItem, dropdownId);
//                    }
//                },
//                change: function () {
//                    const selectedItem = this.dataItem();
//                    if (!selectedItem) {
//                        $("#" + dropdownId).data(widgetType).text("");
//                        $("#" + dropdownId).data(widgetType).value("");
//                    }
//                }
//            };

//            // Merge user-provided options with defaults
//            const finalOptions = { ...defaultOptions, ...options };

//            // Initialize the widget
//            $("#" + dropdownId)[widgetType](finalOptions);

//            // Trigger additional logic after binding
//            SelectedDropdown(dropdownId);

//            // Adjust width if necessary
//            if (widgetType === "kendoComboBox") {
//                AutoWithBindComboBox(dropdownId);
//            } else if (widgetType === "kendoMultiColumnComboBox") {
//                AutoWithBindMultiColumnComboBox(dropdownId);
//            } else if (widgetType === "kendoAutoComplete") {
//                AutoWithBindAutoCompleteComboBox(dropdownId);
//            }
//        },
//        error: function (xhr, status, error) {
//            console.error(`Error fetching data for ${widgetType}:`, error);
//            $("#" + dropdownId)[widgetType]({
//                dataSource: [],
//                placeholder: "Select..."
//            });
//        }
//    });
//}

//function AutoWithBindComboBox(dropdownId) {
//    const widget = $("#" + dropdownId).data("kendoComboBox");
//    const listWidth = widget.list.width();
//    if (listWidth > 100) {
//        widget.list.width("auto");
//    }
//}

//function AutoWithBindMultiColumnComboBox(dropdownId) {
//    const widget = $("#" + dropdownId).data("kendoMultiColumnComboBox");
//    const listWidth = widget.list.width();
//    if (listWidth > 50) {
//        widget.list.width("auto");
//    }
//}

//function AutoWithBindAutoCompleteComboBox(dropdownId) {
//    const widget = $("#" + dropdownId).data("kendoAutoComplete");
//    const listWidth = widget.list.width();
//    if (listWidth > 100) {
//        widget.list.width("auto");
//    }
//}

//const comboBoxUrl = '@Url.Action("GetComboBoxData", "Lookup")';
//BindKendoWidget(comboBoxUrl, "cmb_example", "name", "id", "kendoComboBox");

//const multiColumnComboBoxUrl = '@Url.Action("GetMultiColumnData", "Lookup")';
//const columnsProperties = [
//    { field: "code", title: "Code", width: 100 },
//    { field: "name", title: "Name", width: 200 }
//];
//const filterFields = ["code", "name"];
//BindKendoWidget(
//    multiColumnComboBoxUrl,
//    "cmb_multicolumn",
//    "name",
//    "id",
//    "kendoMultiColumnComboBox",
//    {
//        columns: columnsProperties,
//        footerTemplate: 'Total #: instance.dataSource.total() # items found',
//        filterFields: filterFields
//    }
//);

//const autoCompleteUrl = '@Url.Action("GetAutoCompleteData", "Lookup")';
//BindKendoWidget(autoCompleteUrl, "ac_example", "name", "id", "kendoAutoComplete");