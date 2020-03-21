declare class Choices {
    ajax(callback: any): any;
    constructor(element: HTMLElement, options: ChoicesOptions);
    hideDropdown(): void;
    setChoices(callback: any): void;
    getValue(val?: boolean): any;
    disable(): void;
    removeActiveItemsByValue(value: number): void;
    setChoiceByValue(value: number): void;
    enable(): void;
}

declare interface ChoicesOptions {
    maxItemCount?: number;
    removeItemButton?: boolean;
    duplicateItemsAllowed?: boolean;
    paste?: boolean;
    searchFields?: string[];
    shouldSort?: boolean;
    placeholderValue?: string;
    searchPlaceholderValue?: string;
    searchResultLimit?: number;
    loadingText?: string;
    noResultsText?: string;
    noChoicesText?: string;
    itemSelectText?: string;
    maxItemText?: any;
    fuseOptions?: any;
}