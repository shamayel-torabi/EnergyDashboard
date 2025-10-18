import React from 'react';

class ObjectViewer extends React.Component {
    renderObject = (o) => {
        if (o) {
            return Object.keys(o).map((name, k) => {
                let value = o[name];
                if (Array.isArray(value))
                    return this.renderArrayObject(name, value);
                else if (value === Object(value)) 
                    return this.renderObject(value);
                else 
                    return this.renderSimpleObject(name, value);
            })

        }
        else
            return null;
    }

    renderSimpleObject = (name, value)=> {
        return (
            <li key={name}>
                {name}:{value}
            </li>
        );
    }

    renderArrayObject = (name, obj) => {
        let items = obj.map((n, k) => {
            return this.renderObject(obj[k]);
        })
        return (
            <ul key={name}>
                {name}:{items}
            </ul>
        )
    }

    render() {
        let a = this.renderObject(this.props.object);
        return (
            <ul>
                {a}
            </ul>
        );
    }
}

export default ObjectViewer;