// Google Maps implementation for booking page - shows campsite and accommodation spots
let bookingMap = null;
let bookingPageMarkers = [];

function initializeBookingMap(campsiteLat, campsiteLng, campsiteName, spotsJson) {
    console.log("initializeBookingMap called");
    console.log(`Campsite: ${campsiteName} at ${campsiteLat}, ${campsiteLng}`);
    
    // Check if Google Maps is loaded
    if (typeof google === 'undefined' || !google.maps) {
        console.error("Google Maps API is not loaded!");
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

    // Clear existing markers
    bookingPageMarkers.forEach(marker => marker.setMap(null));
    bookingPageMarkers = [];
    
    // Create map if not already created
    if (!bookingMap) {
        console.log("Creating new Google Map...");
        
        bookingMap = new google.maps.Map(mapContainer, {
            center: { lat: campsiteLat, lng: campsiteLng },
            zoom: 17,
            mapTypeId: 'satellite',
            mapTypeControl: true,
            mapTypeControlOptions: {
                style: google.maps.MapTypeControlStyle.HORIZONTAL_BAR,
                position: google.maps.ControlPosition.TOP_RIGHT
            },
            streetViewControl: false,
            fullscreenControl: true
        });
    } else {
        // Update map center
        bookingMap.setCenter({ lat: campsiteLat, lng: campsiteLng });
    }
    
    // Add campsite marker (blue)
    const campsiteMarker = new google.maps.Marker({
        position: { lat: campsiteLat, lng: campsiteLng },
        map: bookingMap,
        title: campsiteName,
        icon: {
            path: google.maps.SymbolPath.CIRCLE,
            scale: 12,
            fillColor: '#2196F3',
            fillOpacity: 1,
            strokeColor: '#ffffff',
            strokeWeight: 3
        },
        label: {
            text: '⛺',
            fontSize: '18px'
        }
    });
    
    const campsiteInfoWindow = new google.maps.InfoWindow({
        content: `
            <div style="padding: 8px; min-width: 200px;">
                <h3 style="margin: 0 0 8px 0; color: #1976d2; font-size: 16px;">${campsiteName}</h3>
                <p style="margin: 0; color: #666; font-size: 13px;">Main Campsite Location</p>
            </div>
        `
    });
    
    campsiteMarker.addListener('click', () => {
        campsiteInfoWindow.open(bookingMap, campsiteMarker);
    });

    bookingPageMarkers.push(campsiteMarker);

    // Add spot markers
    const bounds = new google.maps.LatLngBounds();
    bounds.extend({ lat: campsiteLat, lng: campsiteLng });
    
    spots.forEach(spot => {
        const isAvailable = spot.status === "Available";
        const markerColor = isAvailable ? '#4caf50' : '#f44336'; // Green for available, red for occupied
        
        const marker = new google.maps.Marker({
            position: { lat: spot.latitude, lng: spot.longitude },
            map: bookingMap,
            title: `${spot.spotId} - ${spot.type}`,
            icon: {
                path: google.maps.SymbolPath.CIRCLE,
                scale: 10,
                fillColor: markerColor,
                fillOpacity: 1,
                strokeColor: '#ffffff',
                strokeWeight: 2
            },
            label: {
                text: spot.spotId,
                color: '#ffffff',
                fontSize: '11px',
                fontWeight: 'bold'
            }
        });
        
        // Create info window content
        const statusBadge = isAvailable 
            ? '<span style="background: #4caf50; color: white; padding: 3px 10px; border-radius: 12px; font-size: 11px; font-weight: bold;">AVAILABLE</span>'
            : '<span style="background: #f44336; color: white; padding: 3px 10px; border-radius: 12px; font-size: 11px; font-weight: bold;">OCCUPIED</span>';
        
        const amenitiesList = spot.amenities && spot.amenities.length > 0
            ? spot.amenities.map(a => `<span style="background: #e3f2fd; color: #1976d2; padding: 2px 8px; border-radius: 8px; font-size: 10px; margin-right: 4px; display: inline-block; margin-top: 2px;">${a}</span>`).join('')
            : '<span style="color: #999; font-size: 11px;">No amenities</span>';
        
        const priceModifierText = spot.priceModifier > 1.0
            ? `<div style="margin-top: 8px; color: #ff9800; font-weight: bold; font-size: 12px;">Premium Spot +${((spot.priceModifier - 1) * 100).toFixed(0)}%</div>`
            : '';
        
        const infoWindow = new google.maps.InfoWindow({
            content: `
                <div style="padding: 8px; min-width: 220px;">
                    <h3 style="margin: 0 0 8px 0; color: #333; font-size: 16px;">Spot ${spot.spotId}</h3>
                    <div style="margin-bottom: 8px;">${statusBadge}</div>
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
            `
        });
        
        marker.addListener('click', () => {
            infoWindow.open(bookingMap, marker);
        });

        bookingPageMarkers.push(marker);
        bounds.extend({ lat: spot.latitude, lng: spot.longitude });
    });
    
    // Fit map to show all markers
    bookingMap.fitBounds(bounds);
    
    console.log(`Booking map initialized with ${spots.length} spots + 1 campsite marker`);
}

// Make function globally available
window.initializeBookingMap = initializeBookingMap;

