// Global variables for the map
window.leafletMap = null;
window.leafletMarkers = [];
window.leafletMarkerLayer = null;

// Make function globally accessible
window.initializeMap = function(campsitesJson) {
    try {
        console.log("Initializing Leaflet map with campsites:", campsitesJson);

        const campsites = JSON.parse(campsitesJson);
        console.log("Parsed campsites:", campsites);

        // Initialize map if not already created
        if (!window.leafletMap) {
            console.log("Creating new Leaflet map...");

            // Create map centered on Denmark
            window.leafletMap = L.map('map').setView([56.2639, 10.4515], 7);

            // Add Esri World Imagery satellite layer (free, no API key required)
            L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', {
                attribution: 'Tiles &copy; Esri &mdash; Source: Esri, i-cubed, USDA, USGS, AEX, GeoEye, Getmapping, Aerogrid, IGN, IGP, UPR-EGP, and the GIS User Community',
                maxZoom: 18,
                minZoom: 6
            }).addTo(window.leafletMap);

            // Add street labels on top of satellite imagery (optional - makes it easier to read)
            L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager_only_labels/{z}/{x}/{y}{r}.png', {
                attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors &copy; <a href="https://carto.com/attributions">CARTO</a>',
                maxZoom: 19,
                minZoom: 6
            }).addTo(window.leafletMap);

            console.log("Leaflet map created successfully with satellite imagery");
        }

        // Clear existing markers
        if (window.leafletMarkerLayer) {
            window.leafletMap.removeLayer(window.leafletMarkerLayer);
        }
        window.leafletMarkers = [];

        // Create a layer group for markers
        window.leafletMarkerLayer = L.layerGroup().addTo(window.leafletMap);

        // Add markers for each campsite
        console.log("Adding markers...");

        campsites.forEach(campsite => {
            // Log campsite data for debugging
            console.log("Processing campsite:", campsite.name, "Lat:", campsite.latitude, "Lng:", campsite.longitude);

            // Create custom icon for campsite
            const campsiteIcon = L.divIcon({
                className: 'custom-campsite-marker',
                html: `
                    <div style="position: relative;">
                        <svg width="32" height="40" viewBox="0 0 32 40" xmlns="http://www.w3.org/2000/svg">
                            <!-- Pin shape -->
                            <path d="M16 0 C7.163 0 0 7.163 0 16 C0 24 16 40 16 40 S32 24 32 16 C32 7.163 24.837 0 16 0 Z"
                                  fill="#d32f2f"
                                  stroke="#fff"
                                  stroke-width="2"/>
                            <!-- Tent icon -->
                            <path d="M16 8 L10 18 L22 18 Z" fill="#fff"/>
                            <circle cx="16" cy="12" r="1.5" fill="#fff"/>
                        </svg>
                    </div>
                `,
                iconSize: [32, 40],
                iconAnchor: [16, 40],
                popupAnchor: [0, -40]
            });

            // Create marker - handle both PascalCase and camelCase property names
            const lat = campsite.latitude || campsite.Latitude || 0;
            const lng = campsite.longitude || campsite.Longitude || 0;

            console.log("Creating marker at:", lat, lng);

            const marker = L.marker([lat, lng], {
                icon: campsiteIcon,
                title: campsite.name || campsite.Name
            });

            // Create popup content with campsite details
            const popupContent = `
                <div style="min-width: 250px;">
                    <h3 style="margin: 0 0 8px 0; color: #1976d2; font-size: 16px;">${campsite.name}</h3>
                    <p style="margin: 4px 0; color: #666; font-size: 13px;">
                        <strong>📍 Region:</strong> ${campsite.region}
                    </p>
                    <p style="margin: 4px 0; font-size: 13px;">${campsite.description}</p>
                    <p style="margin: 8px 0 4px 0; color: #2e7d32; font-size: 13px;">
                        <strong>✅ Available:</strong> ${campsite.availableSpots} spots
                    </p>
                    <div style="margin: 8px 0;">
                        <strong style="font-size: 12px;">🏕️ Accommodation:</strong><br/>
                        <div style="margin-top: 4px;">
                            ${campsite.accommodationTypes.map(type => 
                                `<span style="display: inline-block; background: #e3f2fd; padding: 2px 8px; margin: 2px; border-radius: 12px; font-size: 11px;">${type}</span>`
                            ).join('')}
                        </div>
                    </div>
                    <div style="margin: 8px 0;">
                        <strong style="font-size: 12px;">⭐ Amenities:</strong><br/>
                        <div style="margin-top: 4px; font-size: 11px; color: #666;">
                            ${campsite.amenities.slice(0, 5).join(', ')}${campsite.amenities.length > 5 ? '...' : ''}
                        </div>
                    </div>
                    <button onclick="window.location.href='/booking?campsiteId=${campsite.id}'" 
                            style="width: 100%; margin-top: 8px; padding: 8px; background: #1976d2; color: white; border: none; border-radius: 4px; cursor: pointer; font-size: 13px; font-weight: 500;">
                        Book Now
                    </button>
                </div>
            `;

            marker.bindPopup(popupContent, {
                maxWidth: 300,
                className: 'campsite-popup'
            });

            // Add marker to layer group
            marker.addTo(window.leafletMarkerLayer);
            window.leafletMarkers.push(marker);
        });

        console.log("Added", window.leafletMarkers.length, "markers to map");

        // Adjust map bounds to show all markers if there are any
        if (window.leafletMarkers.length > 0) {
            const group = L.featureGroup(window.leafletMarkers);
            window.leafletMap.fitBounds(group.getBounds().pad(0.1));
        } else {
            // Reset to Denmark view if no markers
            window.leafletMap.setView([56.2639, 10.4515], 7);
        }

    } catch (error) {
        console.error("Error initializing Leaflet map:", error);
    }
};

console.log("leaflet-map.js loaded successfully");

