import React, { Component } from 'react';
import PropTypes from 'prop-types';

import './ContextMenu.scss';

class ContextMenu extends Component {

    constructor(props) {
        super(props);
        this.state = {
            visible: false,
        }

        this.menu = React.createRef();
    }

    static defaultProps = {
        onAction: (item) => { },
    }

    static propTypes = {
        target: PropTypes.string,
        menuItems: PropTypes.array,
        onAction: PropTypes.func,
    };


    componentDidMount() {
        let target = this.props.target;
        let element = document.querySelector(target);

        if (element) {
            element.addEventListener("contextmenu",  this.handleContextMenu);
            document.addEventListener("click", this.handleClick);
            document.addEventListener('scroll', this.handleScroll);
        }
    }

    componentWillUnmount() {
        let target = this.props.target;
        let element = document.querySelector(target);
        if (element) {
            element.removeEventListener("contextmenu", this.handleContextMenu);
            document.removeEventListener('click', this.handleClick);
            document.removeEventListener('scroll', this.handleScroll);
        }
    }

    handleClick = () => {
        const { visible } = this.state;

        if (visible && this.menu.current) {
            this.menu.current.style.display = "none";
            this.setState({ visible: false, });
        }
    }

    handleScroll = () => {
        const { visible } = this.state;
        if (visible) this.setState({ visible: false, });
    }

    handleContextMenu = (event) => {
        event.preventDefault();
        let menu = this.menu.current;


        menu.style.display = "block";
        this.setState({ visible: true, });

        const clickX = event.clientX;
        const clickY = event.clientY;
        //const screenW = window.innerWidth;
        const screenH = window.innerHeight;
        const rootW = menu.offsetWidth;
        const rootH = menu.offsetHeight;

        const left = clickX > rootW;
        const right = !left;
        const top = (screenH - clickY) > rootH;
        const bottom = !top;

        if (right) {
            menu.style.left = `${clickX + 5}px`;
        }

        if (left) {
            menu.style.left = `${clickX - rootW - 5}px`;
        }

        if (top) {
            menu.style.top = `${clickY + 5}px`;
        }

        if (bottom) {
            menu.style.top = `${clickY - rootH - 5}px`;
        }
    }

    handleContexMenuClick = (item) => {
        let menu = this.menu.current;

        if (this.props.onAction)
            this.props.onAction(item);

        const { visible } = this.state;

        if (visible) {
            menu.style.display = "none";
            this.setState({ visible: false, });
        }
    }

    renderMenuItem() {
        let items = this.props.menuItems;

        let menu = items.map((item, k) => {
            if(item.isAdmin)
                return <li id={item.id} key={k} className="menu-option" onClick={(e) => this.handleContexMenuClick(item)}>{item.text}</li>
            else
                return <li id={item.id} key={k} className="menu-option-disable" disabled>{item.text}</li>;
        })

        return menu;
    }

    render() {
        let menu = this.renderMenuItem();
        return (
            <div ref={this.menu} className="menu">
                <ul className="menu-options">
                    {menu}
                </ul>
            </div>
        );
    }
}

export { ContextMenu }

