import React, { useEffect, useState } from 'react';
import './header.css';

function Header() {
    const [show, setShow] = useState(true);
    const [Y, setY] = useState(0);
    const [language, setLanguage] = useState("NL");

    useEffect(() => {
        const handleScroll = () => {
            if (window.scrollY > Y) {
                setShow(false);
            } else {
                setShow(true);
            }
    
            setY(window.scrollY);
        };

        window.addEventListener('scroll', handleScroll);

        return () => {
            window.removeEventListener('scroll', handleScroll)
        }
    }, [Y])

    return <>
        <div id='body-header' className={show ? 'show' : 'hide'}>
            <div id='left'>
                {/*<img id='logo' src='/images/logo_transparent.png'></img>*/}
                <strong id='initials-header'>JDV</strong>
            </div>
            <div id='right'>
                <div id='language-selector'>
                    <button id='language-button'>{language}</button>
                    <div id='language-content'>
                        <button>NL</button>
                        <button>EN</button>
                    </div>
                </div>
            </div>
        </div>
    </>
}

export default Header