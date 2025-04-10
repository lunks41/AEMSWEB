function convertToUpperCase(inputElement) {
    inputElement.value = inputElement.value.trim().toUpperCase();
}

//// toast notification
//toastr.options = {
//    "closeButton": true,
//    "debug": false,
//    "newestOnTop": true,
//    "progressBar": true,
//    "positionClass": "toast-top-right",
//    "preventDuplicates": false,
//    "onclick": null,
//    "showDuration": "300",
//    "hideDuration": "1000",
//    "timeOut": "5000",
//    "extendedTimeOut": "1000",
//    "showEasing": "swing",
//    "hideEasing": "linear",
//    "showMethod": "fadeIn",
//    "hideMethod": "fadeOut"
//}

toastr.options = {
    "closeButton": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "timeOut": "5000",
    "iconClasses": {
        info: 'toast-info',
        success: 'toast-success',
        warning: 'toast-warning',
        error: 'toast-error'
    }
};