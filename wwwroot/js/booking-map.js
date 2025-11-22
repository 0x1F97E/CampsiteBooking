// Leaflet map for booking page - shows individual accommodation spots
let map = null;
let markerLayer = null;

export function initializeBookingMap(spotsJson) {
    console.log("initializeBookingMap called");

    // Check if Leaflet is loaded
    if (typeof L === 'undefined') {
        console.error("Leaflet library is not loaded!");
        return;
    }

    // Check if map container exists
    const mapContainer = document.getElementById('booking-map');
    if (!mapContainer) {
        console.error("Map container 'booking-map' not found in DOM!");
        return;
    }

    const spots = JSON.parse(spotsJson);
    console.log(`Parsed ${spots.length} spots from JSON`);

    if (!spots || spots.length === 0) {
        console.log("No spots to display on map");
        return;
    }

    // Clear existing markers if any
    if (markerLayer) {
        markerLayer.clearLayers();
    } else {
        markerLayer = L.layerGroup();
    }

    // Initialize map if not already created
    if (!map) {
        console.log("Creating new Leaflet map...");

        // Calculate center from spots
        const avgLat = spots.reduce((sum, s) => sum + s.latitude, 0) / spots.length;
        const avgLng = spots.reduce((sum, s) => sum + s.longitude, 0) / spots.length;

        console.log(`Map center: ${avgLat}, ${avgLng}`);

        map = L.map('booking-map').setView([avgLat, avgLng], 16);
        
        // Add Esri World Imagery satellite layer
        L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', {
            attribution: 'Tiles &copy; Esri',
            maxZoom: 19,
            minZoom: 14
        }).addTo(map);
        
        // Add street labels overlay
        L.tileLayer('https://{s}.basemaps.cartocdn.com/rastertiles/voyager_only_labels/{z}/{x}/{y}{r}.png', {
            attribution: '&copy; OpenStreetMap contributors',
            maxZoom: 19,
            minZoom: 14
        }).addTo(map);
        
        markerLayer.addTo(map);
    }
    
    // Create markers for each spot
    const bounds = [];
    
    spots.forEach(spot => {
        const isAvailable = spot.status === "Available";
        const markerColor = isAvailable ? '#4caf50' : '#f44336'; // Green for available, red for occupied
        
        // Create custom marker icon with spot number
        const markerIcon = L.divIcon({
            className: 'custom-spot-marker',
            html: `
                <div style="position: relative;">
                    <svg width="32" height="40" viewBox="0 0 32 40" xmlns="http://www.w3.org/2000/svg">
                        <path d="M16 0C7.163 0 0 7.163 0 16c0 8.837 16 24 16 24s16-15.163 16-24C32 7.163 24.837 0 16 0z" 
                              fill="${markerColor}" 
                              stroke="#fff" 
                              stroke-width="2"/>
                        <circle cx="16" cy="16" r="8" fill="#fff"/>
                    </svg>
                    <div style="position: absolute; top: 8px; left: 50%; transform: translateX(-50%); 
                                font-weight: bold; font-size: 11px; color: ${markerColor}; 
                                text-shadow: 0 0 2px white;">
                        ${spot.spotId}
                    </div>
                </div>
            `,
            iconSize: [32, 40],
            iconAnchor: [16, 40],
            popupAnchor: [0, -40]
        });
        
        // Create marker
        const marker = L.marker([spot.latitude, spot.longitude], {
            icon: markerIcon,
            title: `${spot.spotId} - ${spot.type}`
        });
        
        // Create popup content
        const statusBadge = isAvailable 
            ? '<span style="background: #4caf50; color: white; padding: 2px 8px; border-radius: 12px; font-size: 11px; font-weight: bold;">AVAILABLE</span>'
            : '<span style="background: #f44336; color: white; padding: 2px 8px; border-radius: 12px; font-size: 11px; font-weight: bold;">OCCUPIED</span>';
        
        const amenitiesList = spot.amenities && spot.amenities.length > 0
            ? spot.amenities.map(a => `<span style="background: #e3f2fd; color: #1976d2; padding: 2px 6px; border-radius: 8px; font-size: 10px; margin-right: 4px;">${a}</span>`).join('')
            : '<span style="color: #999; font-size: 11px;">No amenities listed</span>';
        
        const priceModifierText = spot.priceModifier > 1.0
            ? `<div style="margin-top: 8px; color: #ff9800; font-weight: bold; font-size: 12px;">Premium Spot +${((spot.priceModifier - 1) * 100).toFixed(0)}%</div>`
            : '';
        
        const popupContent = `
            <div class="spot-popup" style="min-width: 200px;">
                <div style="font-weight: bold; font-size: 16px; margin-bottom: 8px; color: #333;">
                    Spot ${spot.spotId}
                </div>
                <div style="margin-bottom: 8px;">
                    ${statusBadge}
                </div>
                <div style="margin-bottom: 6px;">
                    <strong style="font-size: 12px;">Type:</strong> 
                    <span style="font-size: 12px;">${spot.type}</span>
                </div>
                <div style="margin-bottom: 6px;">
                    <strong style="font-size: 12px;">Amenities:</strong><br/>
                    <div style="margin-top: 4px;">${amenitiesList}</div>
                </div>
                ${priceModifierText}
            </div>
        `;
        
        marker.bindPopup(popupContent);
        marker.addTo(markerLayer);
        
        bounds.push([spot.latitude, spot.longitude]);
    });
    
    // Fit map to show all spots
    if (bounds.length > 0) {
        map.fitBounds(bounds, { padding: [50, 50] });
    }
    
    console.log(`Booking map initialized with ${spots.length} spots`);
}

