

$(document).ready(function () {
    jQuery.noConflict();
        $.fn.editable.defaults.mode = 'inline';
        $('.myeditable').editable({
            url: '/Home/AddExpertOpinion',
            success: function (response, newValue) {
                if (response.status == 'error') return response.msg;
            }
        });
    });

  

