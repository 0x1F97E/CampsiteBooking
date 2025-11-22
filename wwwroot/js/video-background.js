// Video Background Initialization
(function() {
    'use strict';

    function initVideoBackground() {
        console.log('🎬 Initializing video background...');

        // Create video background container
        const container = document.createElement('div');
        container.className = 'video-background';

        // Create video element
        const video = document.createElement('video');
        video.autoplay = true;
        video.loop = true;
        video.muted = true;
        video.playsInline = true;
        video.preload = 'auto';

        // Create source element
        const source = document.createElement('source');
        source.src = '/sunlight-shines.mp4';
        source.type = 'video/mp4';

        video.appendChild(source);
        container.appendChild(video);

        // Add to body
        document.body.insertBefore(container, document.body.firstChild);

        // Event listeners
        video.addEventListener('loadeddata', function() {
            console.log('✅ Video loaded successfully');
        });

        video.addEventListener('error', function(e) {
            console.error('❌ Video failed to load:', e);
        });

        // Attempt to play
        video.play().then(function() {
            console.log('▶️ Video playing successfully');
        }).catch(function(error) {
            console.warn('⚠️ Video autoplay prevented:', error);
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initVideoBackground);
    } else {
        initVideoBackground();
    }

    // Re-initialize on Blazor navigation (for SPA behavior)
    if (typeof Blazor !== 'undefined') {
        Blazor.addEventListener('enhancedload', initVideoBackground);
    }
})();

