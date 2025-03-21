import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import { createRoot } from 'react-dom/client'
import './index.css'

import Home from './home/home.jsx';
import StudyProgression from './Study/StudyProgression.jsx';

createRoot(document.getElementById('root')).render(
  <Router>
    <Routes>
      <Route path="/StudyProgression" element={<StudyProgression></StudyProgression>}></Route>
      <Route path="/Home" element={<Home></Home>}></Route>
      <Route path="/" element={<Navigate to = "/Home"></Navigate>}></Route>
    </Routes>
  </Router>
)

