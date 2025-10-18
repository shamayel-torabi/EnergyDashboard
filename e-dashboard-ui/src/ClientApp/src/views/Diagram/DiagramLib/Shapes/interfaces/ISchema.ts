interface ITab{
    tabIndex:number,
    tabName:string,
    properties: object;
}

export interface ISchema {
    type:string;
    title: string;
    tabs:ITab[]    
}
