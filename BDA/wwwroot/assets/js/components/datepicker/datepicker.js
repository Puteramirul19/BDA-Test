(function ($) {

    'use strict';
	
    // ------------------------------------------------------- //
    // Datepicker
    // ------------------------------------------------------ //	
	$(function () {
		//default date range picker
		$('#daterange').daterangepicker({
            autoApply: true,
            locale: {
                format: 'DD/MM/YYYY h:mm A' //edited to UK format by Hanif
            }
		});

		//date time picker
		$('#datetime').daterangepicker({
			timePicker: true,
			timePickerIncrement: 30,
			locale: {
				format: 'DD/MM/YYYY h:mm A' //edited to UK format by Hanif
			}
		});

		//single date
		$('#date').daterangepicker({
            singleDatePicker: true,
            locale: {
                format: 'DD/MM/YYYY' //edited to UK format by Hanif
            }

		});
		
		//default date range picker
		$('.daterange').daterangepicker({
            autoApply: true,
            locale: {
                format: 'DD/MM/YYYY h:mm A' //edited to UK format by Hanif
            }
		});

		//date time picker
		$('.datetime').daterangepicker({
			timePicker: true,
			timePickerIncrement: 30,
			locale: {
                format: 'DD/MM/YYYY h:mm A' //edited to UK format by Hanif
                //format: 'MM/DD/YYYY h:mm A'
			}
		});

		//single date
		$('.date').daterangepicker({
            singleDatePicker: true,
            locale: {
                format: 'DD/MM/YYYY' //edited to UK format by Hanif
            }
		});
	});
	
})(jQuery);