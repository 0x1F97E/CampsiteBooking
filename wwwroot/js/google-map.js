// Google Maps initialization for campsite locations
let map = null;
let homePageMarkers = [];
let mapInitialized = false;

// Initialize the map (will be called with data from Blazor component)
window.initMap = function() {
    console.log("⚠️ initMap called without data - waiting for Blazor to provide campsite data");
    // Map will be initialized when Blazor calls initMapWithData() with database data
};

// Initialize the map with provided campsite data
window.initMapWithData = function(campsiteData) {
    console.log("Initializing Google Maps with data:", campsiteData);

    // Check if map container exists
    const mapContainer = document.getElementById("map");
    if (!mapContainer) {
        console.error("Map container not found!");
        return;
    }

    // Clear existing markers if reinitializing
    if (mapInitialized && homePageMarkers.length > 0) {
        homePageMarkers.forEach(m => {
            if (m.marker) m.marker.setMap(null);
        });
        homePageMarkers = [];
    }

    // Create map centered on Denmark, shifted right to cut off water on left
    map = new google.maps.Map(mapContainer, {
        center: { lat: 56.2639, lng: 12.0 },
        zoom: 6.5,
        mapTypeId: 'satellite'
    });

    // Add markers for each campsite
    campsiteData.forEach(campsite => {
        // Handle both naming conventions (lat/lng vs Latitude/Longitude)
        const lat = campsite.lat || campsite.Latitude || 56.2639;
        const lng = campsite.lng || campsite.Longitude || 10.4515;
        const name = campsite.name || campsite.Name || "Campsite";
        const region = campsite.region || campsite.Region || "Denmark";
        const description = campsite.description || campsite.Description || "Beautiful campsite";
        const availableSpots = campsite.availableSpots || campsite.AvailableSpots || 0;
        const id = campsite.id || campsite.Id || 1;

        const marker = new google.maps.Marker({
            position: { lat: lat, lng: lng },
            map: map,
            title: name,
            label: {
                text: availableSpots.toString(),
                color: 'white',
                fontSize: '14px',
                fontWeight: 'bold'
            }
        });

        // Create info window content
        const infoContent = `
            <div style="padding: 10px; min-width: 200px;">
                <h3 style="margin: 0 0 10px 0;">${name}</h3>
                <p style="margin: 5px 0;"><strong>📍 Region:</strong> ${region}</p>
                <p style="margin: 5px 0;">${description}</p>
                <p style="margin: 5px 0;"><strong>✅ Available:</strong> ${availableSpots} spots</p>
                <button onclick="window.location.href='/booking?campsiteId=${id}'"
                        style="margin-top: 10px; padding: 8px 16px; background: #1976d2; color: white; border: none; border-radius: 4px; cursor: pointer;">
                    Book Now
                </button>
            </div>
        `;

        const infoWindow = new google.maps.InfoWindow({
            content: infoContent
        });

        marker.addListener('click', () => {
            // Close all other info windows
            homePageMarkers.forEach(m => {
                if (m.infoWindow) {
                    m.infoWindow.close();
                }
            });

            // Open this info window
            infoWindow.open(map, marker);
        });

        homePageMarkers.push({ marker, infoWindow });
    });

    mapInitialized = true;
    console.log(`Added ${homePageMarkers.length} campsite markers to the map`);
};

