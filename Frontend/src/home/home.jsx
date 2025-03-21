import React, {useState, useEffect} from 'react'
import './home.css'
import Header from '../general/header/header';
import Footer from '../general/footer/footer';
import existingFile from '../utils/existingFile';

function Home() {
    const [language, setLanguage] = useState("NL")

    return <>
        <div className='page-container'>
            <Header></Header>
            <div className='main-content'>
                <img id='image' src='/images/persons/me.jpg' alt='person'></img>

                <div id='about'>
                    <h1><strong>About Me</strong></h1>
                    
                    {language === "NL" ? (
                        <p>
                            <p><strong>Naam: </strong>Jarno Daniël Vis</p>
                        </p>
                    ) : null}

                    {language === "EN" ? (
                        <p><strong>Name: </strong>Jarno Daniël Vis</p>
                    ) : null}
                </div>
            </div>
            <Footer></Footer>
        </div>
    </>
}

export default Home;