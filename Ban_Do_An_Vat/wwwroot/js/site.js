function showToast(message, type = 'success') {
    const container = document.getElementById('toast-container');
    if (!container) return;

    // Create toast element
    const toast = document.createElement('div');
    toast.className = `transform translate-y-2 opacity-0 transition-all duration-300 ease-out pointer-events-auto flex items-center gap-3 px-5 py-3.5 rounded-2xl shadow-lg border text-sm max-w-sm font-semibold`;
    
    // Icon and colors based on type
    let iconClass = 'ph ph-check-circle-bold';
    let themeClass = 'bg-white border-green-200 text-green-800';
    
    if (type === 'error') {
        iconClass = 'ph ph-x-circle-bold';
        themeClass = 'bg-white border-red-200 text-red-800';
    } else if (type === 'warning') {
        iconClass = 'ph ph-warning-circle-bold';
        themeClass = 'bg-white border-yellow-200 text-yellow-800';
    } else if (type === 'info') {
        iconClass = 'ph ph-info-bold';
        themeClass = 'bg-white border-blue-200 text-blue-800';
    }

    toast.className += ` ${themeClass}`;
    toast.innerHTML = `
        <i class="${iconClass} text-xl shrink-0"></i>
        <span>${message}</span>
    `;

    // Append to container
    container.appendChild(toast);

    // Trigger entrance animation
    setTimeout(() => {
        toast.classList.remove('translate-y-2', 'opacity-0');
        toast.classList.add('translate-y-0', 'opacity-100');
    }, 10);

    // Remove toast after 3 seconds
    setTimeout(() => {
        toast.classList.remove('translate-y-0', 'opacity-100');
        toast.classList.add('translate-y-2', 'opacity-0');
        
        // Remove from DOM after transition completes
        toast.addEventListener('transitionend', () => {
            toast.remove();
        });
    }, 3000);
}
