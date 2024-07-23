$(document).ready(function () {
    // Function to load books using AJAX
    function loadBooks() {
        $.ajax({
            url: 'http://localhost:5265/odata/Books', // Your OData API endpoint
            method: 'GET',
            dataType: 'json',
            success: function (data) {
                // Generate HTML for books
                var booksHtml = '';
                $.each(data.value, function (index, book) {
                    booksHtml += `
                        <div class="col-md-4 mb-4">
                            <div class="card">
                                <img src="${book.ImageUrl}" class="card-img-top" alt="${book.Name}" style="height: 200px; object-fit: cover;">
                                <div class="card-body">
                                    <h5 class="card-title">${book.Name}</h5>
                                    <p class="card-text">${book.Description}</p>
                                </div>
                                <div class="card-footer">
                                    <span class="text-muted">${book.Price.toLocaleString()} VND</span>
                                    <a href="/cart/add/${book.Id}" class="btn btn-secondary btn-sm float-right">Đặt hàng</a>
                                </div>
                            </div>
                        </div>
                    `;
                });
                // Insert generated HTML into the container
                $('#booksContainer').html(booksHtml);
            },
            error: function (xhr, status, error) {
                console.error('AJAX Error:', status, error);
            }
        });
    }

    // Load books when the document is ready
    loadBooks();
});
