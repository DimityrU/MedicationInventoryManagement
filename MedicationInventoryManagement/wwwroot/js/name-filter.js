$(document).ready(function () {
    $("#searchFilter").on("input", function () {
        var filterValue = $(this).val().toLowerCase();

        $("tbody tr").each(function () {
            var medicationName = $(this).find("td:first").text().toLowerCase();

            if (medicationName.includes(filterValue)) {
                $(this).show();
            } else {
                $(this).hide();
            }
        });
    });
});
