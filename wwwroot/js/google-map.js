// Google Maps initialization for campsite locations
let map = null;
let homePageMarkers = [];
let mapInitialized = false;

// Campsite data with Danish coordinates
const campsites = [
    {
        id: 1,
        name: "Copenhagen Beach Camp",
        region: "Zealand",
        description: "Beautiful beachside camping near Copenhagen",
        lat: 55.6761,
        lng: 12.5683,
        availableSpots: 45
    },
    {
        id: 2,
        name: "Skagen North Point",
        region: "North Jutland",
        description: "Denmark's northernmost campsite with stunning views",
        lat: 57.7209,
        lng: 10.5882,
        availableSpots: 32
    },
    {
        id: 3,
        name: "Aarhus Forest Retreat",
        region: "East Jutland",
        description: "Peaceful forest camping near Aarhus city",
        lat: 56.1629,
        lng: 10.2039,
        availableSpots: 28
    },
    {
        id: 4,
        name: "Odense Family Camp",
        region: "Funen",
        description: "Family-friendly campsite on Funen island",
        lat: 55.4038,
        lng: 10.4024,
        availableSpots: 52
    },
    {
        id: 5,
        name: "Bornholm Island Camp",
        region: "Bornholm",
        description: "Scenic island camping with rocky coastline",
        lat: 55.1367,
        lng: 14.9155,
        availableSpots: 18
    },
    {
        id: 6,
        name: "Ribe Viking Camp",
        region: "Southwest Jutland",
        description: "Historic campsite near Denmark's oldest town",
        lat: 55.3282,
        lng: 8.7619,
        availableSpots: 38
    },
    {
        id: 7,
        name: "Roskilde Fjord Camp",
        region: "Zealand",
        description: "Waterfront camping by the beautiful Roskilde Fjord",
        lat: 55.6415,
        lng: 12.0803,
        availableSpots: 41
    },
    {
        id: 8,
        name: "Aalborg Limfjord",
        region: "North Jutland",
        description: "Camping by the Limfjord with water activities",
        lat: 57.0488,
        lng: 9.9217,
        availableSpots: 35
    }
];

// Initialize the map with default data (fallback)
window.initMap = function() {
    console.log("Initializing Google Maps with default data...");
    initMapWithData(campsites);
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

    // Create map centered on Denmark
    map = new google.maps.Map(mapContainer, {
        center: { lat: 56.2639, lng: 10.4515 },
        zoom: 7,
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

