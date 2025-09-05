// TweakProg Admin Panel JavaScript

let sidebarCollapsed = false;

// Initialize admin panel
function initializeAdminPanel() {
    setupSidebarToggle();
    setupResponsiveSidebar();
    setupTooltips();
    setupDataTables();
    setupAutoRefresh();
    loadUserInfo();
}

// Sidebar toggle functionality
function setupSidebarToggle() {
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');

    if (sidebarToggle && sidebar && mainContent) {
        sidebarToggle.addEventListener('click', function() {
            sidebarCollapsed = !sidebarCollapsed;
            
            if (sidebarCollapsed) {
                sidebar.classList.add('collapsed');
                mainContent.classList.add('expanded');
            } else {
                sidebar.classList.remove('collapsed');
                mainContent.classList.remove('expanded');
            }
            
            // Save state to localStorage
            localStorage.setItem('sidebarCollapsed', sidebarCollapsed);
        });

        // Load saved state
        const savedState = localStorage.getItem('sidebarCollapsed');
        if (savedState === 'true') {
            sidebarCollapsed = true;
            sidebar.classList.add('collapsed');
            mainContent.classList.add('expanded');
        }
    }
}

// Responsive sidebar behavior
function setupResponsiveSidebar() {
    const sidebar = document.getElementById('sidebar');
    const mainContent = document.getElementById('mainContent');

    function handleResize() {
        if (window.innerWidth <= 768) {
            sidebar.classList.add('collapsed');
            mainContent.classList.add('expanded');
        } else {
            // Only restore if it wasn't manually collapsed
            if (!sidebarCollapsed) {
                sidebar.classList.remove('collapsed');
                mainContent.classList.remove('expanded');
            }
        }
    }

    window.addEventListener('resize', handleResize);
    handleResize(); // Initial check
}

// Setup tooltips
function setupTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Setup data tables with search and pagination
function setupDataTables() {
    const tables = document.querySelectorAll('.data-table');
    tables.forEach(table => {
        if (table) {
            // Add search functionality
            addTableSearch(table);
            // Add pagination
            addTablePagination(table);
        }
    });
}

// Add search functionality to tables
function addTableSearch(table) {
    const searchContainer = document.createElement('div');
    searchContainer.className = 'table-search mb-3';
    searchContainer.innerHTML = `
        <div class="input-group">
            <span class="input-group-text"><i class="fas fa-search"></i></span>
            <input type="text" class="form-control" placeholder="Search..." id="search-${table.id}">
        </div>
    `;
    
    table.parentNode.insertBefore(searchContainer, table);
    
    const searchInput = searchContainer.querySelector('input');
    searchInput.addEventListener('input', function() {
        filterTable(table, this.value);
    });
}

// Filter table rows based on search input
function filterTable(table, searchTerm) {
    const rows = table.querySelectorAll('tbody tr');
    const term = searchTerm.toLowerCase();
    
    rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        if (text.includes(term)) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}

// Add pagination to tables
function addTablePagination(table) {
    const rows = table.querySelectorAll('tbody tr');
    const rowsPerPage = 10;
    const totalPages = Math.ceil(rows.length / rowsPerPage);
    
    if (totalPages <= 1) return;
    
    // Hide all rows initially
    rows.forEach(row => row.style.display = 'none');
    
    // Show first page
    showPage(table, 1, rowsPerPage);
    
    // Create pagination controls
    const paginationContainer = document.createElement('div');
    paginationContainer.className = 'table-pagination mt-3';
    paginationContainer.innerHTML = createPaginationHTML(totalPages);
    
    table.parentNode.appendChild(paginationContainer);
    
    // Add click handlers
    paginationContainer.addEventListener('click', function(e) {
        if (e.target.classList.contains('page-link')) {
            e.preventDefault();
            const page = parseInt(e.target.dataset.page);
            showPage(table, page, rowsPerPage);
            updatePaginationActive(paginationContainer, page);
        }
    });
}

// Show specific page of table rows
function showPage(table, page, rowsPerPage) {
    const rows = table.querySelectorAll('tbody tr');
    const start = (page - 1) * rowsPerPage;
    const end = start + rowsPerPage;
    
    rows.forEach((row, index) => {
        if (index >= start && index < end) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}

// Create pagination HTML
function createPaginationHTML(totalPages) {
    let html = '<nav><ul class="pagination justify-content-center">';
    
    for (let i = 1; i <= totalPages; i++) {
        const active = i === 1 ? 'active' : '';
        html += `<li class="page-item ${active}">
            <a class="page-link" href="#" data-page="${i}">${i}</a>
        </li>`;
    }
    
    html += '</ul></nav>';
    return html;
}

// Update pagination active state
function updatePaginationActive(container, activePage) {
    const pageItems = container.querySelectorAll('.page-item');
    pageItems.forEach(item => {
        item.classList.remove('active');
        if (parseInt(item.querySelector('.page-link').dataset.page) === activePage) {
            item.classList.add('active');
        }
    });
}

// Auto-refresh functionality
function setupAutoRefresh() {
    // Auto-refresh stats every 30 seconds
    setInterval(() => {
        if (typeof loadStats === 'function') {
            loadStats();
        }
    }, 30000);
    
    // Auto-refresh activity logs every 60 seconds
    setInterval(() => {
        if (typeof loadRecentActivity === 'function') {
            loadRecentActivity();
        }
    }, 60000);
}

// Load user information
function loadUserInfo() {
    const userNameElement = document.getElementById('userName');
    if (userNameElement) {
        // You can fetch user info from an API endpoint here
        // For now, we'll use a default value
        userNameElement.textContent = 'Admin User';
    }
}

// Show loading overlay
function showLoading() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        overlay.classList.add('show');
    }
}

// Hide loading overlay
function hideLoading() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) {
        overlay.classList.remove('show');
    }
}

// Show notification
function showNotification(message, type = 'info', duration = 5000) {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
    notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.appendChild(notification);
    
    // Auto-remove after duration
    setTimeout(() => {
        if (notification.parentNode) {
            notification.remove();
        }
    }, duration);
}

// Confirm dialog
function confirmAction(message, callback) {
    if (confirm(message)) {
        callback();
    }
}

// Format date for display
function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
}

// Format file size
function formatFileSize(bytes) {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
}

// Copy to clipboard
function copyToClipboard(text) {
    navigator.clipboard.writeText(text).then(() => {
        showNotification('Copied to clipboard!', 'success', 2000);
    }).catch(() => {
        showNotification('Failed to copy to clipboard', 'danger', 3000);
    });
}

// Export table to CSV
function exportTableToCSV(tableId, filename) {
    const table = document.getElementById(tableId);
    if (!table) return;
    
    let csv = [];
    const rows = table.querySelectorAll('tr');
    
    rows.forEach(row => {
        const cols = row.querySelectorAll('td, th');
        const rowData = [];
        cols.forEach(col => {
            rowData.push('"' + col.textContent.replace(/"/g, '""') + '"');
        });
        csv.push(rowData.join(','));
    });
    
    const csvContent = csv.join('\n');
    const blob = new Blob([csvContent], { type: 'text/csv' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = filename || 'export.csv';
    a.click();
    window.URL.revokeObjectURL(url);
}

// API helper functions
async function apiRequest(url, options = {}) {
    showLoading();
    try {
        const response = await fetch(url, {
            headers: {
                'Content-Type': 'application/json',
                ...options.headers
            },
            ...options
        });
        
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }
        
        return await response.json();
    } catch (error) {
        console.error('API request failed:', error);
        showNotification('Request failed: ' + error.message, 'danger');
        throw error;
    } finally {
        hideLoading();
    }
}

// Initialize when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    initializeAdminPanel();
    
    // Add fade-in animation to page content
    const pageContent = document.querySelector('.page-content');
    if (pageContent) {
        pageContent.classList.add('fade-in');
    }
});

// Global functions for use in views
window.showLoading = showLoading;
window.hideLoading = hideLoading;
window.showNotification = showNotification;
window.confirmAction = confirmAction;
window.formatDate = formatDate;
window.formatFileSize = formatFileSize;
window.copyToClipboard = copyToClipboard;
window.exportTableToCSV = exportTableToCSV;
window.apiRequest = apiRequest;