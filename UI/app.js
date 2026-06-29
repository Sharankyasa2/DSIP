// storing shipment data locally
let shipmentList = [];
// api url
const apiUrl = "https://localhost:7181/api/shipments"; // change to your port
$(document).ready(function () {
    // loading page data
    loadShipments();
    loadStats();
    // form submit
    $("#shipmentForm").submit(bookShipment);
    // tracking shipment
    $("#trackBtn").click(trackShipment);
    // filtering shipment
    $("#statusFilter").change(filterShipments);
});
// showing loader
function showLoader() {
    $("#loader").css("display", "flex");
}
// hiding loader
function hideLoader() {
    $("#loader").hide();
}
// showing notification message
function showNotification(message, type) {
    let notification = $("#notification");
    notification
        .removeClass("success error")
        .addClass(type)
        .text(message)
        .fadeIn();
    setTimeout(function () {
        notification.fadeOut();
    }, 2500);
}
// getting all shipments
function loadShipments() {
    showLoader();
    $.ajax({
        url: apiUrl,
        type: "GET",
        success: function (shipments) {
            shipmentList = shipments;
            renderTable(shipmentList);
        },
        error: function () {
            showNotification("Unable to load shipments.", "error");
        },
        complete: function () {
            hideLoader();
        }
    });
}
// refreshing dashboard data
function refreshDashboard() {
    loadShipments();
    loadStats();
}
// loading dashboard stats
function loadStats() {
    $.ajax({
        url: `${apiUrl}/stats`,
        type: "GET",
        success: function (stats) {
            $("#bookedCount").text(stats.booked);
            $("#inTransitCount").text(stats.inTransit);
            $("#outForDeliveryCount").text(stats.outForDelivery);
            $("#deliveredCount").text(stats.delivered);
            $("#rtoCount").text(stats.rto);
            $("#totalCount").text(stats.total);
        }
    });
}
// booking shipment
// booking new shipment
function bookShipment(e) {
    e.preventDefault();
    let shipment = {
        awbNumber: $("#awbNumber").val(),
        senderName: $("#senderName").val(),
        receiverName: $("#receiverName").val(),
        origin: $("#origin").val(),
        destination: $("#destination").val(),
        weightKg: parseFloat($("#weightKg").val())
    };
    showLoader();
    $.ajax({
        url: apiUrl,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(shipment),
        success: function () {
            showNotification("Shipment booked successfully.", "success");
            $("#shipmentForm")[0].reset();
            refreshDashboard();
        },
        error: function (xhr) {
        let message = "Something went wrong.";
        if (xhr.responseJSON && xhr.responseJSON.message) {
            message = xhr.responseJSON.message;
        }
        showNotification(message, "error");
        },
        complete: function () {
            hideLoader();
        }
    });
}
// filtering shipment by status
function filterShipments() {
    let selectedStatus = $("#statusFilter").val();
    if (selectedStatus === "All") {
        renderTable(shipmentList);
        return;
    }
    let filteredShipment = shipmentList.filter(function (shipment) {
        return shipment.status === selectedStatus;
    });
    renderTable(filteredShipment);
}
// updating shipment status
function updateStatus(awb, status) {
    showLoader();
    $.ajax({
        url: `${apiUrl}/${awb}/status`,
        type: "PUT",
        contentType: "application/json",
        data: JSON.stringify({
            status: status
        }),
        success: function () {
            showNotification("Shipment status updated.", "success");
            refreshDashboard();
        },
        error: function (xhr) {
        let message = "Something went wrong.";
        if (xhr.responseJSON && xhr.responseJSON.message) {
            message = xhr.responseJSON.message;
        }
        showNotification(message, "error");
        },
        complete: function () {
            hideLoader();
        }
    });
}
// rendering table
// displaying shipment data inside table
function renderTable(shipments) {
    let table = $("#shipmentTable");
    table.empty();
    $.each(shipments, function (index, shipment) {
        let row = `
            <tr>
                <td>${shipment.awbNumber}</td>
                <td>${shipment.senderName}</td>
                <td>${shipment.receiverName}</td>
                <td>${shipment.origin} → ${shipment.destination}</td>
                <td>${shipment.weightKg} Kg</td>
                <td>
                    <select
                        class="status-select"
                        data-awb="${shipment.awbNumber}">
                        <option value="Booked" ${shipment.status === "Booked" ? "selected" : ""}>Booked</option>
                        <option value="In Transit" ${shipment.status === "In Transit" ? "selected" : ""}>In Transit</option>
                        <option value="Out for Delivery" ${shipment.status === "Out for Delivery" ? "selected" : ""}>Out for Delivery</option>
                        <option value="Delivered" ${shipment.status === "Delivered" ? "selected" : ""}>Delivered</option>
                        <option value="RTO" ${shipment.status === "RTO" ? "selected" : ""}>RTO</option>
                    </select>
                </td>
                <td>
                    <button
                        class="action-btn delete-btn"
                        data-id="${shipment.shipmentId}">
                        Delete
                    </button>
                </td>
            </tr>
        `;
        table.append(row);
    });
    // status changed
    $(".status-select").change(function () {
        let awb = $(this).data("awb");
        let status = $(this).val();
        updateStatus(awb, status);
    });
    // deleting shipment
    $(".delete-btn").click(function () {
        let shipmentId = $(this).data("id");
        deleteShipment(shipmentId);
    });
}
// deleting shipment
function deleteShipment(id) {
    if (!confirm("Delete this shipment?")) {
        return;
    }
    showLoader();
    $.ajax({
        url: `${apiUrl}/${id}`,
        type: "DELETE",
        success: function () {
            showNotification("Shipment deleted successfully.", "success");
            refreshDashboard();
        },
        error: function (xhr) {
        let message = "Something went wrong.";
        if (xhr.responseJSON && xhr.responseJSON.message) {
            message = xhr.responseJSON.message;
        }
        showNotification(message, "error");
        },
        complete: function () {
            hideLoader();
        }
    });
}
// tracking shipment using awb
function trackShipment() {
    let awb = $("#trackAwb").val().trim();
    if (awb === "") {
        showNotification("Enter AWB Number.", "error");
        return;
    }
    showLoader();
    $.ajax({
        url: `${apiUrl}/${awb}`,
        type: "GET",
        success: function (shipment) {
            $("#trackResult").html(`
            <div class="track-result-card">
                <h3>${shipment.awbNumber}</h3>
                <div class="track-row">
                    <span>Sender</span>
                    <strong>${shipment.senderName}</strong>
                </div>
                <div class="track-row">
                    <span>Receiver</span>
                    <strong>${shipment.receiverName}</strong>
                </div>
                <div class="track-row">
                    <span>Route</span>
                    <strong>${shipment.origin} → ${shipment.destination}</strong>
                </div>
                <div class="track-row">
                    <span>Weight</span>
                    <strong>${shipment.weightKg} kg</strong>
                </div>
                <div class="track-row">
                    <span>Status</span>
                    <span class="${getStatusClass(shipment.status)}">
                        ${shipment.status}
                    </span>
                </div>
            </div>
            `);
        },
        error: function (xhr) {
        let message = "Something went wrong.";
        if (xhr.responseJSON && xhr.responseJSON.message) {
            message = xhr.responseJSON.message;
        }
        showNotification(message, "error");
        },
        complete: function () {
            hideLoader();
        }
    });
}

// getting css class based on shipment status
function getStatusClass(status) {

    switch (status) {

        case "Booked":
            return "status-badge booked";

        case "In Transit":
            return "status-badge in-transit";

        case "Out for Delivery":
            return "status-badge out-for-delivery";

        case "Delivered":
            return "status-badge delivered";

        case "RTO":
            return "status-badge rto";

        default:
            return "status-badge";

    }

}