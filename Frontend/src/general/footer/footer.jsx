import React, { useEffect, useState } from 'react';
import './footer.css';

function Footer() {
    return <>
        <div id='body-footer'>
            <div id='items'>
                <a href="https://www.linkedin.com/in/jarno-vis-590b702a2/"><img src="images/footer/linked_in.png" alt="linked-in"></img></a>
                <a href="https://github.com/Jarnovis"><img src="images/footer/github.png" alt="github"></img></a>
            </div>
        </div>
    </>
}

export default Footer