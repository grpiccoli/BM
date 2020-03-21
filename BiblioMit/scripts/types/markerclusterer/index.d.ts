declare class MarkerClusterer {
    constructor(map: google.maps.Map, opt_markers?: Array<google.maps.Marker>, opt_options?: MarkerClustererOptions);
    setCalculator(callback: any): MarkerClusterer;
    setMap(callback: any): any;
    onRemove(callback: any): any;
    clearMarkers(): any;
    repaint(): any;
}

declare interface MarkerClustererOptions {
    gridSize?: number;
    maxZoom?: number;
    zoomOnClick?: boolean;
    averageCenter?: boolean;
    minimumClusterSize?: number;
    ignoreHiddenMarkers?: boolean;
    cssClass?: string;
    styles?: Array<any>;
    onMouseoverCluster?: any;
    onMouseoutCluster?: any;
    drawCluster?: any;
    hideCluster?: any;
    showCluster?: any;
    onAddCluster?: any;
    onRemoveCluster?: any;
    imagePath?: string;
}