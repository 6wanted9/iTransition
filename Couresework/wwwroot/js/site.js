// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
(function () {
    'use strict'

    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    var forms = document.querySelectorAll('.needs-validation')
    // Loop over them and prevent submission
    Array.prototype.slice.call(forms)
        .forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault()
                    event.stopPropagation()
                }

                form.classList.add('was-validated')
            }, false)
        })
})()
var tags =  Json.Serialize(allTags);
//
$('#drag-and-drop-zone').dmUploader({
    url: '/demo/java-script/upload',
    maxFileSize: 3000000, // 3 Megs max
    allowedTypes: 'image/*',
    extFilter: ['jpg', 'jpeg', 'png', 'gif'],
    // ...
    onNewFile: function (id, file) {
        //...
        if (typeof FileReader !== 'undefined') {
            var reader = new FileReader();
            var img = $('<img />');

            reader.onload = function (e) {
                img.attr('src', e.target.result);
            }
            /* ToDo: do something with the img! */
            reader.readAsDataURL(file);
        }
    },
    onFileTypeError: function (file) {
        // ...
    },
    onFileTypeError: function (file) {
        // ...
    }
    // ...
});