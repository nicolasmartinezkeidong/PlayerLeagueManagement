/// Method to get the first H1 element of each view. If not H1 element was found, do it with the first H2 element

// Wait for the DOM content to load before executing the code
window.addEventListener('DOMContentLoaded', function () {
    // Find the first h1 in current view
    var firstH1 = document.querySelector('main > h1');

    // If no h1, look for the first h2
    if (!firstH1) {
        var firstH2 = document.querySelector('main > h2');

        if (firstH2) {
            // Clone the found h2 to prevent moving it from its original place
            var clonedH2 = firstH2.cloneNode(true);
            var pageHeadersSection = document.querySelector('.page-headers');

            if (pageHeadersSection) {
                // Insert the cloned h2 into the .page-headers section
                pageHeadersSection.appendChild(clonedH2);

                // Remove the original h2 from view
                firstH2.remove();
            }
        }
    } else {
        // Clone the found h1 to prevent moving it from its original place
        var clonedH1 = firstH1.cloneNode(true);
        var pageHeadersSection = document.querySelector('.page-headers');

        if (pageHeadersSection) {
            // Insert the cloned h1 into the .page-headers section
            pageHeadersSection.appendChild(clonedH1);

            // Remove the original h1 from view
            firstH1.remove();
        }
    }
});