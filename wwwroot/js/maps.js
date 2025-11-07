let map;
let markers = [];

function initializeMap(campsitesJson) {
    try {
        console.log("Initializing map with campsites:", campsitesJson);

        const campsites = JSON.parse(campsitesJson);
        console.log("Parsed campsites:", campsites);

        // Clear existing markers
        markers.forEach(marker => marker.setMap(null));
        markers = [];

        // Calculate center of Denmark or center of filtered campsites
        let center = { lat: 56.2639, lng: 10.4515 }; // Denmark center
        let zoom = 7;

        if (campsites.length > 0) {
            // Calculate average position of all campsites
            const avgLat = campsites.reduce((sum, c) => sum + c.latitude, 0) / campsites.length;
            const avgLng = campsites.reduce((sum, c) => sum + c.longitude, 0) / campsites.length;
            center = { lat: avgLat, lng: avgLng };

            if (campsites.length === 1) {
                zoom = 10;
            }
        }

        console.log("Map center:", center, "Zoom:", zoom);

        // Initialize map if not already created
        if (!map) {
            console.log("Creating new map...");
            map = new google.maps.Map(document.getElementById("map"), {
                center: center,
                zoom: zoom
            });
            console.log("Map created successfully");
        } else {
            console.log("Updating existing map...");
            map.setCenter(center);
            map.setZoom(zoom);
        }

        // Add markers for each campsite
        console.log("Adding markers...");

        campsites.forEach(campsite => {
            const marker = new google.maps.Marker({
                map: map,
                position: { lat: campsite.latitude, lng: campsite.longitude },
                title: campsite.name,
                icon: {
                    path: google.maps.SymbolPath.CIRCLE,
                    scale: 10,
                    fillColor: "#1976d2",
                    fillOpacity: 1,
                    strokeColor: "#ffffff",
                    strokeWeight: 2
                }
            });

            // Add click listener to show info window
            marker.addListener("click", () => {
                showCampsiteInfo(campsite);
            });

            markers.push(marker);
        });

        console.log("Added", markers.length, "markers to map");
    } catch (error) {
        console.error("Error initializing map:", error);
    }
}

function showCampsiteInfo(campsite) {
    // This will be called from Blazor to show the info card
    console.log("Selected campsite:", campsite);
}

