
export class CRUDService {
    constructor(url, accessToken) {
        this.baseUrl = url;
        this.accessToken = accessToken;
    }

    readAll = async () => {
        const url = this.baseUrl
        let data = null;

        let req;
        if (this.accessToken) {
            req = new Request(url, {
                method: 'Get',
                headers: new Headers({
                    'Authorization': ` Bearer ${this.accessToken}`
                })
            })
        }
        else {
            req = new Request(url)
        }

        try {
            let response = await fetch(req);

            if (response.ok){
                data = await response.json();
            }
            else if (response.status === 401) {
                const problem = {
                    title : `برای به روز رسانی باید به سایت وارد شوید`
                }
                throw new Error(problem);
            }
            else {
                const problem = await response.json();
                throw new Error(problem);
            }
            return data;
        }
        catch (e) {
            console.log('CRUDService getAll error', e);
        }
    }

    read = async (id) => {
        const url =`${this.baseUrl}/${id}`;
        let data = null;

        let req;
        if (this.accessToken) {
            req = new Request(url, {
                method: 'Get',
                headers: new Headers({
                    'Authorization': ` Bearer ${this.accessToken}`
                })
            })
        }
        else {
            req = new Request(url)
        }

        try {
            let response = await fetch(req);

            if (response.ok){
                data = await response.json();
            }
            else if (response.status === 401) {
                const problem = {
                    title : `برای به روز رسانی باید به سایت وارد شوید`
                }
                throw new Error(problem);
            }
            else {
                const problem = await response.json();
                throw new Error(problem);
            }
            return data;
        }
        catch (e) {
            console.log('CRUDService get error', e);
        }
    }

    update = async (id, diagram) => {
        const url =`${this.baseUrl}/${id}`;
        let data = null;

        let req;
        if (this.accessToken) {
            req = new Request(url, {
                method: 'PUT',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Authorization': ` Bearer ${this.accessToken}`
                }),
                body: JSON.stringify(diagram)
            })
        }
        else {
            req = new Request(url, {
                method: 'PUT',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                }),
                body: JSON.stringify(diagram)
            })
        }

        try {
            let response = await fetch(req);

            if (response.ok){
                data = await response.json();
            }
            else if (response.status === 401) {
                const problem = {
                    title : `برای به روز رسانی باید به سایت وارد شوید`,
                    status: 401
                }
                throw new Error(problem);
            }
            else {
                const problem = await response.json();
                throw new Error(problem);
            }
        }
        catch (e) {
            console.log('CRUDService update error', e);
        }
        return data;
    }

    create = async (diagram) => {
        const url = this.baseUrl;
        let data = null;

        let req;
        if (this.accessToken) {
            req = new Request(url, {
                method: 'POST',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json',
                    'Authorization': ` Bearer ${this.accessToken}`
                }),
                body: JSON.stringify(diagram)
            })
        }
        else {
            req = new Request(url, {
                method: 'POST',
                headers: new Headers({
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                }),
                body: JSON.stringify(diagram)
            })
        }

        try {
            let response = await fetch(req);

            if (response.ok) {
                data = await response.json();
            }
            else if (response.status === 401) {
                const problem = {
                    title : `برای به روز رسانی باید به سایت وارد شوید`,
                    status: 401
                }
                throw new Error(problem);
            }
            else {
                const problem = await response.json();
                console.error(problem);
                throw new Error(problem);
            }
        }
        catch (e) {
            console.log('CRUDService create error', e);
        }
        return data;
    }

    delete = async (id) => {
        const url =`${this.baseUrl}/${id}`;
        let data = null;

        let req;
        if (this.accessToken) {
            req = new Request(url, {
                method: 'DELETE',
                headers: new Headers({
                    'Authorization': ` Bearer ${this.accessToken}`
                })
            })
        }
        else {
            req = new Request(url, {
                method: 'DELETE'
            })
        }

        try {
            let response = await fetch(req);

            if (response.ok){
                data = await response.json();
            }
            else if (response.status === 401) {
                const problem = {
                    title : `برای به روز رسانی باید به سایت وارد شوید`,
                    status: 401
                }
                throw new Error(problem);
            }
            else {
                const problem = await response.json();
                throw new Error(problem);
            }
        }
        catch (e) {
            console.log('CRUDService delete error', e);
        }

        return data;
    }
}