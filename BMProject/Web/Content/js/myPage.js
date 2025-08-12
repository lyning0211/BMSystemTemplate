// 全局分页参数
let currentPage = 1;
let pageSize = 10;
let totalCount = 0;
let totalPages = 0;

// 渲染整个分页区域
function renderPager() {
    if (totalCount === 0) {
        $("#paginationContainer").html('<div class="pagination-info text-c">没有记录</div>');
        return;
    }

    // 计算记录范围
    const start = (currentPage - 1) * pageSize + 1;
    const end = Math.min(currentPage * pageSize, totalCount);

    // 构建分页控件的HTML
    let paginationHtml = '';

    // 添加上一页
    const prevClass = currentPage === 1 ? "disabled" : "";
    paginationHtml += `
                <li class="${prevClass}">
                    <a href="javascript:;" onclick="changePage(${currentPage - 1})">«</a>
                </li>
            `;

    // 智能页码显示算法
    let startPage, endPage;
    const maxVisiblePages = 5; // 最多显示的页码数量

    if (totalPages <= maxVisiblePages) {
        // 总页数较少，显示所有页码
        startPage = 1;
        endPage = totalPages;
    } else {
        // 计算起始和结束页码
        const halfVisible = Math.floor(maxVisiblePages / 2);

        if (currentPage <= halfVisible + 1) {
            // 当前页在开始位置
            startPage = 1;
            endPage = maxVisiblePages;
        } else if (currentPage >= totalPages - halfVisible) {
            // 当前页在结束位置
            startPage = totalPages - maxVisiblePages + 1;
            endPage = totalPages;
        } else {
            // 当前页在中间位置
            startPage = currentPage - halfVisible;
            endPage = currentPage + halfVisible;
        }
    }

    // 添加第一页（如果不在当前显示范围）
    if (startPage > 1) {
        paginationHtml += `
                    <li>
                        <a href="javascript:;" onclick="changePage(1)">1</a>
                    </li>
                `;

        // 添加省略号（如果前面还有更多页）
        if (startPage > 2) {
            paginationHtml += `
                        <li class="disabled">
                            <a href="javascript:;">...</a>
                        </li>
                    `;
        }
    }

    // 添加页码
    for (let i = startPage; i <= endPage; i++) {
        const active = i === currentPage ? "active" : "";
        paginationHtml += `
                    <li class="${active}">
                        <a href="javascript:;" onclick="changePage(${i})">${i}</a>
                    </li>
                `;
    }

    // 添加最后一页（如果不在当前显示范围）
    if (endPage < totalPages) {
        // 添加省略号（如果后面还有更多页）
        if (endPage < totalPages - 1) {
            paginationHtml += `
                        <li class="disabled">
                            <a href="javascript:;">...</a>
                        </li>
                    `;
        }

        paginationHtml += `
                    <li>
                        <a href="javascript:;" onclick="changePage(${totalPages})">${totalPages}</a>
                    </li>
                `;
    }

    // 添加下一页
    const nextClass = currentPage === totalPages ? "disabled" : "";
    paginationHtml += `
                <li class="${nextClass}">
                    <a href="javascript:;" onclick="changePage(${currentPage + 1})">»</a>
                </li>
            `;

    // 构建整个分页区域的HTML
    const html = `
                <div class="pagination-area">
                    <div class="pagination-container">
                        <div class="pagination-info">
                            显示第 ${start} 到 ${end} 条记录，共 ${totalCount} 条记录
                        </div>
                        
                        <div class="pagination-controls">
                            <ul class="pagination">
                                ${paginationHtml}
                            </ul>
                        </div>
                        
                        <div class="page-size-selector">
                            <span>每页显示:</span>
                            <select id="pageSizeSelect">
                                <option value="10" ${pageSize === 10 ? 'selected' : ''}>10</option>
                                <option value="50" ${pageSize === 50 ? 'selected' : ''}>50</option>
                                <option value="100" ${pageSize === 100 ? 'selected' : ''}>100</option>
                                <option value="200" ${pageSize === 200 ? 'selected' : ''}>200</option>
                            </select>
                            <span>&nbsp;条</span>
                        </div>
                    </div>
                </div>
            `;

    // 插入到容器
    $("#paginationContainer").html(html);

    // 绑定每页数量选择器事件
    $("#pageSizeSelect").change(function () {
        pageSize = parseInt($(this).val());
        currentPage = 1;
        loadView();
    });
}

// 分页切换函数
function changePage(page) {
    if (page < 1 || page > totalPages) return;
    currentPage = page;
    loadView();

    // 滚动到表格顶部
    $('html, body').animate({
        scrollTop: $(".table-container").offset().top - 100
    }, 400);
}