document.getElementById("addMedicationLineButton").addEventListener("click", (e) => {
    e.preventDefault();
    var medicationsContainer = document.getElementById("medicationsContainer");
    var newLineIndex = medicationsContainer.getElementsByClassName("row mb-3").length;

    var newLineHTML =
        `<div class="row mb-3">
            <div class="form-group col-md-6">
                <input type="text" name="Order.OrderMedication[${newLineIndex}].Medication.MedicationName" class="form-control">
                <select id="select${newLineIndex}" name="Order.OrderMedication[${newLineIndex}].Medication.MedicationId" class="form-control hide" required></select>
                <input type="checkbox" id="sameBatchCheckbox${newLineIndex}" onclick="toggleMedicationInput(this)">
                <label for="sameBatchCheckbox${newLineIndex}">Same Batch</label>
            </div>
            <div class="form-group col-md-4">
                <input name="Order.OrderMedication[${newLineIndex}].newQuantity" class="form-control" type="number" min="30" max="100" placeholder="30-100" required>
            </div>
            <div class="form-group col-md-1">
                <button type="button" class="btn btn-danger" onclick="removeMedicationLine(this)">X</button>
            </div>
        </div>`;

    medicationsContainer.insertAdjacentHTML("beforeend", newLineHTML);

    var newSelect = document.getElementById(`select${newLineIndex}`);
    selectListItems.forEach(item => {
        var option = document.createElement("option");
        option.value = item.value;
        option.text = item.text;
        newSelect.appendChild(option);
    });
});

function removeMedicationLine(button) {
    button.parentElement.parentElement.remove();
}

function toggleMedicationInput(checkbox) {
    var medicationSelect = checkbox.parentElement.querySelector('select');
    var medicationInput = checkbox.parentElement.querySelector('input[type="text"]');
    if (checkbox.checked) {
        medicationSelect.style.display = 'block';
        medicationInput.style.display = 'none';
    } else {
        medicationSelect.style.display = 'none';
        medicationInput.style.display = 'block';
    }
}