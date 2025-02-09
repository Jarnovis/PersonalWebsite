import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { createRoot } from 'react-dom/client'
import './index.css'

// Import pages
import StudyProgression from './Study/StudyProgression.jsx';


createRoot(document.getElementById('root')).render(
  <Router>
    <Routes>
      <Route path="/StudyProgression" element={<StudyProgression></StudyProgression>}></Route>
    </Routes>
  </Router>
)
