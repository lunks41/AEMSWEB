function initializeKendoGrid(gridId, url, params, columns, height = 600) {
    $("#" + gridId).kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: url,
                    dataType: "json",
                    // Map Kendo's parameters to your server's parameters
                    data: function (options) {
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
        scrollable: {
            virtual: true // Use virtual scrolling for large datasets
        },
        columnMenu: {
            filterable: false
        },
        filterable: { mode: "row" },
        sortable: true,
        navigatable: true,
        resizable: true,
        reorderable: true,
        pageable: {
            refresh: true,
            pageSizes: [20, 50, 100, 1000],
        },
        toolbar: ["excel", "pdf", "search"],
        pdfExport: function (e) {
            const width = e.sender.wrapper.width();
            e.sender.wrapperClone.width(width);
            e.sender.wrapperClone.addClass('k-clone');
        },
        columns: columns,
        height: height // Dynamically set grid height
    });
}
function initializeKendoGridWithoutPaging(gridId, url, params, columns, height = 600) {
    $("#" + gridId).kendoGrid({
        dataSource: {
            transport: {
                read: {
                    url: url,
                    dataType: "json",
                    data: function (options) {
                        return {
                            ...params,
                        };
                    }
                }
            },
            schema: {
                data: "data" // Adjust this based on your JSON structure
            },
        },
        columnMenu: false, // Disables column menu
        navigatable: true, // Enables keyboard navigation
        resizable: true, // Allows resizing of columns
        reorderable: true, // Allows reordering of columns
        toolbar: ["search"],
        columns: columns,
        height: height // Dynamically set grid height
    });
}
function initializeKendoTreeList(treeListId, url, params, columns, height = 600) {
    $("#" + treeListId).kendoTreeList({
        dataSource: {
            transport: {
                read: {
                    url: url,
                    dataType: "json",
                    data: function (options) {
                        return {
                            ...params, // Static parameters
                            parentId: options.id || null, // Current node ID for hierarchical data
                            level: options.level || 0 // Node depth level
                        };
                    }
                }
            },
            serverPaging: false, // Typically false for hierarchical data
            serverFiltering: true,
            serverSorting: true,
            autoSync: true,
            schema: {
                model: {
                    id: "id", // Unique identifier field
                    parentId: "parentId", // Parent identifier field
                    fields: {
                        id: { type: "number" }, // Configure field types
                        parentId: { type: "number", nullable: true },
                        // Add other fields from your schema
                        expanded: { type: "boolean", defaultValue: true }
                    },
                    expanded: true // Initial expansion state
                },
                data: "data", // Response data field
                total: "total" // Response total field
            }
        },
        columns: [
            // Hierarchy column should be first
            {
                field: "name", // Your main display field
                title: "Name",
                template: function (dataItem) {
                    // Add indentation based on hierarchy level
                    const indent = dataItem.level * 30;
                    return `<div style="padding-left:${indent}px">${dataItem.name}</div>`;
                }
            },
            ...columns // Additional columns
        ],
        filterable: {
            mode: "row" // Row filter mode
        },
        sortable: true,
        resizable: true,
        reorderable: true,
        toolbar: ["excel", "pdf", "search"],
        pdfExport: function (e) {
            const width = e.sender.wrapper.width();
            e.sender.wrapperClone.width(width);
            e.sender.wrapperClone.addClass('k-clone');
        },
        height: height,
        // TreeList specific configurations
        autoBind: true,
        loadOnDemand: false, // Set true for lazy loading
        dataBound: function (e) {
            // Optional: Expand all nodes after data binding
            this.expand(".k-treelist-row");
        },
        error: function (e) {
            console.error("TreeList error:", e.xhr.responseText);
        }
    });
}
function bindAutoComplete(url, dropdownId, textField) {

    if ($("#" + dropdownId).data("kendoAutoComplete")) {
        debugger;
        $("#" + dropdownId).siblings(".k-clear-value").remove();
        $("#" + dropdownId).data("kendoAutoComplete").destroy();
    }
    $("#" + dropdownId).empty();

    $.ajax({
        url: url,
        async: false, // Synchronous request (deprecated - consider using async/await)
        type: "GET",
        cache: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            try {
                // Parse response data
                const data = typeof response === "string" ? JSON.parse(response) : response;

                // Initialize Kendo AutoComplete
                $("#" + dropdownId).kendoAutoComplete({
                    dataTextField: textField,
                    filter: "contains",
                    dataSource: data || [],
                });

                // Integrated width adjustment logic
                const autoComplete = $("#" + dropdownId).data("kendoAutoComplete");
                if (autoComplete) {
                    const listWidth = autoComplete.list.width();
                    if (listWidth > 100) {
                        autoComplete.list.width("auto");
                    }
                }
            } catch (error) {
                console.error("Error initializing Kendo AutoComplete:", error);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data for AutoComplete:", error);
            $("#" + dropdownId).kendoAutoComplete({
                dataSource: [],
                placeholder: "Select...",
                filter: "contains"
            });
        }
    });
}
function bindComboBox(url, dropdownId, textField, valueField) {
    try {
        debugger;
        // Cleanup previous instances
        if ($("#" + dropdownId).data("kendoComboBox")) {
            debugger;
            $("#" + dropdownId).siblings(".k-clear-value").remove();
            $("#" + dropdownId).data("kendoComboBox").destroy();
        }
        $("#" + dropdownId).empty();

        $.ajax({
            url: url,
            async: false,
            type: "GET",
            cache: false,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                try {
                    debugger;
                    const data = typeof response === "string" ? JSON.parse(response) : response;
                    // Initialize Kendo ComboBox
                    $("#" + dropdownId).kendoComboBox({
                        dataTextField: textField,
                        dataValueField: valueField,
                        placeholder: "Select...",
                        filter: "contains",
                        dataSource: data || [],
                        select: function (e) {
                            const selectedItem = this.dataItem(e.item.index());
                            if (selectedItem) {
                                // Store the selected item ID to track changes
                                $("#" + dropdownId).data("selectedValue", selectedItem[valueField]);
                                OnSelectDropdown(selectedItem, dropdownId);
                            }
                        },
                        change: function () {
                            const comboBox = $("#" + dropdownId).data("kendoComboBox");
                            const selectedItem = comboBox.dataItem();

                            if (!selectedItem) {
                                comboBox.value(""); // Reset if invalid selection
                                return;
                            }

                            // Get previously selected value
                            const prevSelectedValue = $("#" + dropdownId).data("selectedValue");

                            // Prevent duplicate call if value didn't change
                            if (prevSelectedValue === selectedItem[valueField]) {
                                return;
                            }

                            // Update stored value and call OnSelectDropdown
                            $("#" + dropdownId).data("selectedValue", selectedItem[valueField]);
                            OnSelectDropdown(selectedItem, dropdownId);
                        }
                    });

                    // Adjust dropdown width to "auto" if needed
                    const comboBox = $("#" + dropdownId).data("kendoComboBox");
                    if (comboBox) {
                        const listWidth = comboBox.list.width();
                        if (listWidth > 100) {
                            comboBox.list.width("auto");
                        }
                    }

                    SelectedDropdown(dropdownId);
                } catch (error) {
                    console.error("Error initializing Kendo ComboBox:", error);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching data for ComboBox:", error);
                $("#" + dropdownId).kendoComboBox({
                    dataSource: [],
                    placeholder: "Select...",
                    filter: "contains"
                });
            }
        });

        

    } catch (e) {
        console.error("Unexpected error:", e);
    }
}
function bindMultiColumnComboBox(url, dropdownId, textField, valueField, columnsProperties, filterFields) {

    if ($("#" + dropdownId).data("kendoMultiColumnComboBox")) {
        debugger;
        $("#" + dropdownId).siblings(".k-clear-value").remove();
        $("#" + dropdownId).data("kendoMultiColumnComboBox").destroy();
    }
    $("#" + dropdownId).empty();

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
                    dataSource: data || [],
                    columns: columnsProperties || [],
                    footerTemplate: 'Total #: instance.dataSource.total() # items found',
                    filterFields: filterFields || [],
                    autoWidth: true,
                    dropDownWidth: "auto",
                    htmlAttributes: { class: "wide-combobox" },
                    select: function (e) {
                        const selectedItem = this.dataItem(e.item.index());
                        if (selectedItem) {
                            OnSelectDropdown(selectedItem, dropdownId);
                        }
                    },
                    change: function () {
                        const selectedItem = this.dataItem();
                        if (!selectedItem) {
                            const comboBox = $("#" + dropdownId).data("kendoMultiColumnComboBox");
                            comboBox.text("");
                            comboBox.value("");
                        } else {
                            OnSelectDropdown(selectedItem, dropdownId);
                        }
                    }
                });

                // Integrated width adjustment logic
                const comboBox = $("#" + dropdownId).data("kendoMultiColumnComboBox");
                if (comboBox) {
                    const listWidth = comboBox.list.width();
                    if (listWidth > 100) {
                        comboBox.list.width("auto");
                    }
                }

                // Trigger additional logic after binding
                SelectedDropdown(dropdownId);
            } catch (error) {
                console.error("Error initializing Kendo MultiColumnComboBox:", error);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching data for MultiColumnComboBox:", error);
            $("#" + dropdownId).kendoMultiColumnComboBox({
                dataSource: [],
                placeholder: "Select...",
                filter: "contains"
            });
        }
    });
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