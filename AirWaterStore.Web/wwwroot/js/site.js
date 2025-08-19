// AirWaterStore - Enhanced JavaScript
(function () {
    'use strict';

    // Initialize tooltips and popovers
    document.addEventListener('DOMContentLoaded', function () {
        // Initialize Bootstrap tooltips
        var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });

        // Initialize Bootstrap popovers
        var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
        var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
            return new bootstrap.Popover(popoverTriggerEl);
        });

        // Add smooth scrolling to all links
        document.querySelectorAll('a[href^="#"]').forEach(anchor => {
            anchor.addEventListener('click', function (e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });

        // Add loading states to forms
        initializeFormLoadingStates();
        
        // Add smooth animations
        initializeAnimations();
        
        // Add cart interactions
        initializeCartInteractions();
    });

    // Form loading states
    function initializeFormLoadingStates() {
        document.querySelectorAll('form').forEach(form => {
            form.addEventListener('submit', function () {
                const submitBtn = this.querySelector('button[type="submit"]');
                if (submitBtn) {
                    const originalText = submitBtn.innerHTML;
                    submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>Processing...';
                    submitBtn.disabled = true;
                    
                    // Re-enable after a delay (in case of validation errors)
                    setTimeout(() => {
                        submitBtn.innerHTML = originalText;
                        submitBtn.disabled = false;
                    }, 5000);
                }
            });
        });
    }

    // Smooth animations
    function initializeAnimations() {
        // Intersection Observer for fade-in animations
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver((entries) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('fade-in-up');
                }
            });
        }, observerOptions);

        // Observe elements with animation classes
        document.querySelectorAll('.card, .alert, .btn').forEach(el => {
            observer.observe(el);
        });

        // Add hover effects to cards
        document.querySelectorAll('.card').forEach(card => {
            card.addEventListener('mouseenter', function () {
                this.style.transform = 'translateY(-8px)';
                this.style.boxShadow = '0 12px 40px rgba(0, 0, 0, 0.15)';
            });

            card.addEventListener('mouseleave', function () {
                this.style.transform = 'translateY(0)';
                this.style.boxShadow = '0 8px 32px rgba(0, 0, 0, 0.1)';
            });
        });
    }

    // Cart interactions
    function initializeCartInteractions() {
        // Quantity input validation
        document.querySelectorAll('input[type="number"]').forEach(input => {
            input.addEventListener('change', function () {
                const value = parseInt(this.value);
                const min = parseInt(this.min) || 1;
                const max = parseInt(this.max) || 99;
                
                if (value < min) {
                    this.value = min;
                } else if (value > max) {
                    this.value = max;
                }
            });
        });

        // Add to cart confirmation
        document.querySelectorAll('form[asp-page-handler="AddToCart"]').forEach(form => {
            form.addEventListener('submit', function () {
                const btn = this.querySelector('button[type="submit"]');
                const originalText = btn.innerHTML;
                
                btn.innerHTML = '<i class="fas fa-check me-2"></i>Added!';
                btn.classList.remove('btn-primary');
                btn.classList.add('btn-success');
                
                setTimeout(() => {
                    btn.innerHTML = originalText;
                    btn.classList.remove('btn-success');
                    btn.classList.add('btn-primary');
                }, 2000);
            });
        });
    }

    // Utility functions
    window.AirWaterStore = {
        // Show toast notification
        showToast: function (message, type = 'info') {
            const toast = document.createElement('div');
            toast.className = `alert alert-${type} alert-dismissible fade show position-fixed`;
            toast.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
            toast.innerHTML = `
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            `;
            
            document.body.appendChild(toast);
            
            // Auto-remove after 5 seconds
            setTimeout(() => {
                if (toast.parentNode) {
                    toast.remove();
                }
            }, 5000);
        },

        // Format currency
        formatCurrency: function (amount, locale = 'vi-VN', currency = 'VND') {
            return new Intl.NumberFormat(locale, {
                style: 'currency',
                currency: currency
            }).format(amount);
        },

        // Debounce function
        debounce: function (func, wait) {
            let timeout;
            return function executedFunction(...args) {
                const later = () => {
                    clearTimeout(timeout);
                    func(...args);
                };
                clearTimeout(timeout);
                timeout = setTimeout(later, wait);
            };
        },

        // Smooth scroll to element
        scrollToElement: function (element, offset = 0) {
            const elementPosition = element.getBoundingClientRect().top;
            const offsetPosition = elementPosition + window.pageYOffset - offset;
            
            window.scrollTo({
                top: offsetPosition,
                behavior: 'smooth'
            });
        }
    };

    // Search functionality enhancement
    const searchInput = document.querySelector('input[name="searchString"]');
    if (searchInput) {
        const debouncedSearch = AirWaterStore.debounce(function (value) {
            // Add search suggestions or live search functionality here
            console.log('Searching for:', value);
        }, 300);

        searchInput.addEventListener('input', function () {
            debouncedSearch(this.value);
        });
    }

    // Add loading spinner to page transitions
    window.addEventListener('beforeunload', function () {
        document.body.style.opacity = '0.5';
    });

    // Enhance form validation feedback
    document.querySelectorAll('.form-control').forEach(input => {
        // Remove any existing validation classes on page load
        input.classList.remove('is-valid', 'is-invalid');
        
        input.addEventListener('blur', function () {
            // Only show validation styling if the user has actually interacted with the field
            if (this.dataset.touched === 'true') {
                // Remove any existing validation classes first
                this.classList.remove('is-valid', 'is-invalid');
                
                if (this.checkValidity()) {
                    this.classList.add('is-valid');
                } else {
                    this.classList.add('is-invalid');
                }
            }
        });

        input.addEventListener('input', function () {
            // Mark field as touched when user starts typing
            this.dataset.touched = 'true';
            
            // Remove validation styling when user is typing
            this.classList.remove('is-valid', 'is-invalid');
        });

        input.addEventListener('focus', function () {
            // Mark field as touched when user focuses
            this.dataset.touched = 'true';
        });

        // Prevent browser default validation styling
        input.addEventListener('invalid', function(e) {
            e.preventDefault();
        });
    });

    // Add parallax effect to hero sections
    window.addEventListener('scroll', function () {
        const scrolled = window.pageYOffset;
        const parallaxElements = document.querySelectorAll('.hero-section');
        
        parallaxElements.forEach(element => {
            const speed = 0.5;
            element.style.transform = `translateY(${scrolled * speed}px)`;
        });
    });

    // Initialize lazy loading for images
    if ('IntersectionObserver' in window) {
        const imageObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    const img = entry.target;
                    img.src = img.dataset.src;
                    img.classList.remove('lazy');
                    imageObserver.unobserve(img);
                }
            });
        });

        document.querySelectorAll('img[data-src]').forEach(img => {
            imageObserver.observe(img);
        });
    }

    // Add keyboard navigation support
    document.addEventListener('keydown', function (e) {
        // Ctrl/Cmd + K for search focus
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            const searchInput = document.querySelector('input[name="searchString"]');
            if (searchInput) {
                searchInput.focus();
            }
        }

        // Escape key to close modals/alerts
        if (e.key === 'Escape') {
            const openAlert = document.querySelector('.alert.show');
            if (openAlert) {
                const closeBtn = openAlert.querySelector('.btn-close');
                if (closeBtn) {
                    closeBtn.click();
                }
            }
        }
    });

    // Performance optimization: Throttle scroll events
    let ticking = false;
    function updateOnScroll() {
        // Add scroll-based animations here
        ticking = false;
    }

    window.addEventListener('scroll', function () {
        if (!ticking) {
            requestAnimationFrame(updateOnScroll);
            ticking = true;
        }
    });

})();
