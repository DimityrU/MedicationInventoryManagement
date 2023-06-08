document.getElementById("addMedicationLineButton").addEventListener("click", (e) =>
{
    e.preventDefault();
    var medicationsContainer = document.getElementById("medicationsContainer");
    var newLineIndex = medicationsContainer.getElementsByClassName("row").length;

    var newLineHTML = `<div class="row mb-3">
                <div class="form-group col-md-6">
                    <input type="text" name="Order.Medication[${newLineIndex}].Medication.MedicationName" class="form-control" placeholder="Medication Name" required>
                </div>
                <div class="form-group col-md-6">
                    <input type="number" name="Order.Medication[${newLineIndex}].newQuantity" class="form-control" min="30" max="100" placeholder="30-100" required>
                </div>
            </div>`;

    medicationsContainer.innerHTML += newLineHTML;
});