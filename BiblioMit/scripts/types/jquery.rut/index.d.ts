/// <reference types="jquery"/>

interface JQueryStatic {
    validateRut(
        rut: string,
        callback: (r: number, dv: string) => void,
        object: { minimumLength: number }) : boolean;
}

interface JQuery<TElement = HTMLElement> {
    rut(object: {
        formatOn: string,
        minimumLength?: number,
        validateOn?: string
    }): this;
}