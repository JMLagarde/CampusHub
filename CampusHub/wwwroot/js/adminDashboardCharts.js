window.adminDashboard = {
    categoryChart: null,
    collegeChart: null,

    init: function () {
        this.initWithData(null);
    },

    initWithData: function (dashboardStats) {
        if (typeof Chart === 'undefined') {
            console.error('Chart.js is not loaded');
            return;
        }

        // Destroy existing charts
        if (this.categoryChart) {
            this.categoryChart.destroy();
            this.categoryChart = null;
        }
        if (this.collegeChart) {
            this.collegeChart.destroy();
            this.collegeChart = null;
        }

        // Wait a bit for DOM to be ready
        setTimeout(() => {
            this.renderCharts(dashboardStats);
        }, 100);
    },

    renderCharts: function (dashboardStats) {
        if (!dashboardStats) {
            return;
        }

        // === CATEGORY CHART ===
        const categoryEl = document.getElementById('categoryChart');

        if (categoryEl) {
            const categoryDist = dashboardStats.CategoryDistribution || dashboardStats.categoryDistribution;

            if (categoryDist && Array.isArray(categoryDist) && categoryDist.length > 0) {
                try {
                    const ctx = categoryEl.getContext('2d');
                    const colors = ['#6CE22D', '#4CAF50', '#8BC34A', '#CDDC39', '#9E9E9E', '#FF9800', '#E91E63', '#9C27B0', '#3F51B5', '#2196F3'];

                    const labels = categoryDist.map(item => item.Category || item.category);
                    const data = categoryDist.map(item => item.Percentage || item.percentage);

                    this.categoryChart = new Chart(ctx, {
                        type: 'doughnut',
                        data: {
                            labels: labels,
                            datasets: [{
                                data: data,
                                backgroundColor: colors.slice(0, labels.length),
                                borderWidth: 0
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            plugins: {
                                legend: { display: false },
                                tooltip: {
                                    callbacks: {
                                        label: function (context) {
                                            return context.label + ': ' + context.parsed + '%';
                                        }
                                    }
                                }
                            }
                        }
                    });
                } catch (error) {
                    console.error('Error creating category chart:', error);
                }
            }
        }

        // === COLLEGE CHART ===
        const collegeEl = document.getElementById('collegeChart');

        if (collegeEl) {
            const collegeDist = dashboardStats.CollegeDistribution || dashboardStats.collegeDistribution;

            if (collegeDist && Array.isArray(collegeDist) && collegeDist.length > 0) {
                try {
                    const ctx = collegeEl.getContext('2d');

                    const labels = collegeDist.map(item => item.College || item.college);
                    const data = collegeDist.map(item => item.Count || item.count);

                    this.collegeChart = new Chart(ctx, {
                        type: 'bar',
                        data: {
                            labels: labels,
                            datasets: [{
                                label: 'Students',
                                data: data,
                                backgroundColor: '#6CE22D',
                                borderRadius: 4
                            }]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: false,
                            plugins: { legend: { display: false } },
                            scales: {
                                y: {
                                    beginAtZero: true,
                                    grid: { color: '#E5E7EB' },
                                    ticks: { precision: 0 }
                                },
                                x: {
                                    grid: { display: false },
                                    ticks: {
                                        maxRotation: 45,
                                        minRotation: 45,
                                        font: { size: 10 }
                                    }
                                }
                            }
                        }
                    });
                } catch (error) {
                    console.error('Error creating college chart:', error);
                }
            }
        }
    }
};