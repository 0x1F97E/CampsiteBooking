// Seasonal Video Background
//
// HOW TO GET FREE VIDEOS:
// 1. Go to https://www.pexels.com/videos/ or https://coverr.co/
// 2. Search for: "spring nature", "summer beach", "autumn leaves", "winter snow"
// 3. Download the video you like
// 4. Place it in wwwroot/videos/ folder (create if doesn't exist)
// 5. Update the URLs below to point to your local videos: '/videos/spring.mp4'
//
// OR use direct Pexels URLs (they work without downloading):
// - Right-click on a video → Inspect → Find the <video> tag → Copy the src URL
//
(function() {
    'use strict';

    // Free video URLs from Pexels (royalty-free, no attribution required)
    // REPLACE THESE URLs with your own videos for better performance
    const seasonalVideos = {
        // Spring - Cherry blossoms / flowers blooming / green nature
        spring: 'https://videos.pexels.com/video-files/4620142/4620142-uhd_2560_1440_24fps.mp4',

        // Summer - Sunny beach / ocean waves / bright sunshine
        summer: 'https://videos.pexels.com/video-files/1409899/1409899-hd_1920_1080_24fps.mp4',

        // Autumn - Falling leaves / orange forest / autumn colors
        autumn: 'https://videos.pexels.com/video-files/3843433/3843433-uhd_2560_1440_25fps.mp4',

        // Winter - Snowfall / snowy landscape / frost
        winter: 'https://videos.pexels.com/video-files/3044191/3044191-uhd_2560_1440_25fps.mp4'
    };

    // Get current season based on month
    function getCurrentSeason() {
        const month = new Date().getMonth() + 1; // 1-12

        if (month >= 3 && month <= 5) return 'spring';      // March-May
        if (month >= 6 && month <= 8) return 'summer';      // June-August
        if (month >= 9 && month <= 11) return 'autumn';     // September-November
        return 'winter';                                     // December-February
    }

    // Create video background container
    function createVideoBackground() {
        // Remove existing background if any
        const existing = document.querySelector('.video-background');
        if (existing) {
            existing.remove();
        }

        const container = document.createElement('div');
        container.className = 'video-background';
        document.body.insertBefore(container, document.body.firstChild);
        return container;
    }

    // Create and load video element
    function createVideoElement(videoUrl) {
        const video = document.createElement('video');
        video.autoplay = true;
        video.loop = true;
        video.muted = true;
        video.playsInline = true;
        video.preload = 'auto';
        video.crossOrigin = 'anonymous'; // Allow CORS

        const source = document.createElement('source');
        source.src = videoUrl;
        source.type = 'video/mp4';

        video.appendChild(source);

        // Add active class after video loads
        video.addEventListener('loadeddata', function() {
            console.log('✅ Video loaded successfully');
            video.classList.add('active');
        });

        // Error handling
        video.addEventListener('error', function(e) {
            console.error('❌ Video failed to load:', e);
            console.error('Video URL:', videoUrl);
        });

        // Fallback: add active class after a delay if loadeddata doesn't fire
        setTimeout(function() {
            if (!video.classList.contains('active')) {
                console.log('⚠️ Video not loaded yet, forcing active class');
                video.classList.add('active');
            }
        }, 2000);

        return video;
    }

    // Initialize seasonal video background
    function initSeasonalAnimation() {
        console.log('🎬 Starting seasonal animation initialization...');

        const season = getCurrentSeason();
        console.log(`🌍 Current season: ${season}`);

        const container = createVideoBackground();
        console.log('📦 Video container created');

        const videoUrl = seasonalVideos[season];
        console.log(`📹 Video URL: ${videoUrl}`);

        const video = createVideoElement(videoUrl);
        container.appendChild(video);
        console.log('🎥 Video element added to container');

        // Attempt to play (some browsers require user interaction)
        video.play().then(function() {
            console.log('▶️ Video playing successfully');
        }).catch(function(error) {
            console.warn('⚠️ Video autoplay prevented:', error);
            console.log('Video will still show as a static frame');
        });
    }

    // Initialize when DOM is ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSeasonalAnimation);
    } else {
        initSeasonalAnimation();
    }

    // Re-initialize on Blazor navigation (for SPA behavior)
    window.addEventListener('load', function() {
        // Check periodically if we need to reinitialize (for Blazor navigation)
        setInterval(function() {
            if (!document.querySelector('.video-background')) {
                initSeasonalAnimation();
            }
        }, 2000);
    });

    // Expose function globally for manual initialization
    window.initSeasonalAnimation = initSeasonalAnimation;
})();

