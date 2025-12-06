// File download helper for Blazor
window.downloadFileFromStream = async (fileName, base64Content) => {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,' + base64Content;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};

