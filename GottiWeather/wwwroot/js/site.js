var errorOccured = false;
$(document).ready(pageReady);

function pageReady() {
    // Get location only if cookie is not defined 
    // This avoids calling AccuWeather API again, being more cost-effective:
    //   AccuWeather Standard Pricing Package includes 225,000 calls per month
    //   This is enough for less than 10,000 users using the app for 2 hours a day
    //   (assuming the refresh time is 5 minutes)
    if (!errorOccured && Cookies.get('GottiWeather.Location.Key') == undefined) {
        getLocation();
    }
}

function ressetPosition() {
    removeCookieByName("GottiWeather.Location", true);
    removeCookieByName("GottiWeather.Latitude", false);
    removeCookieByName("GottiWeather.Longitude", false);
    getLocation();
}

function getLocation() {
    var options = {
        enableHighAccuracy: true,
        timeout: 5000,
        maximumAge: 0
    };

    if (navigator.geolocation) {
        var geo = navigator.geolocation.getCurrentPosition(sendPosition, getLocationError, options);
    } else {
        showMessageModal("Error", "Geolocation is not supported by this browser.");
    }

}

function sendPosition(position) {
    var inMinutes = ((1 / 24) / 60) * 2;

    Cookies.set('GottiWeather.Latitude', position.coords.latitude, { expires: inMinutes });
    Cookies.set('GottiWeather.Longitude', position.coords.longitude, { expires: inMinutes });

    location.reload(true);
}

function setLanguage(language) {
    var data = { language: language };
    $.post("/Home/SetLanguage", data);
}

function getLocationError(err) {
    showMessageModal("Error", err.message);
};


function removeCookieByName(cookieNameToExclude, startsWith) {
    if (startsWith == undefined) {
        startsWith = false;
    }

    Object.keys(Cookies.get()).forEach(function (cookieName) {
        var neededAttributes = {
            // Here you pass the same attributes that were used when the cookie was created
            // and are required when removing the cookie
        };
        if (startsWith) {
            if (cookieName.startsWith(cookieNameToExclude)) {
                Cookies.remove(cookieName, neededAttributes);
            }
        } else {
            if (cookieNameToExclude == cookieName) {
                Cookies.remove(cookieName, neededAttributes);
            }
        }
    });
}

function showMessageModal(title, message) {
    $("#messageModalTitle").html(title);
    $("#messageModalText").html(message);
    $("#messageModal").modal();
}

function showViewModelErrorMessage() {
    $("#messageModalTitle").html("Error");
    $("#messageModal").modal();
}

