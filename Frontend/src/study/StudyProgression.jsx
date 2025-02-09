import React, { useEffect, useState } from 'react';
import { pull } from '../utils/fetcher'
import ProgressionCircle from '../study/ProgressionCircle';
import SubjectCard from '../study/SubjectCard';
import './StudyProgression.css';

const BACKEND_URL = import.meta.env.APP_BACKEND_URL ?? 'http://localhost:5072';

function calculateDegreePercentage(degree)
{
    return degree.currentPoints / degree.totalPoints * 100;
}

function StudyProgression()
{
    const [degrees, setDegrees] = useState([]);
    const [subjects, setSubjects] = useState([]);
    const [loading, setLoading] = useState(true);
    const [selectedSubject, setSelectedSubject] = useState(null); 

    useEffect(() => {
        const loadDegrees = async () => {
            const response = await pull(`${BACKEND_URL}/api/StudyInfo/GetAllDegrees`);

            setDegrees(response.degrees);
            await loadSubjects(response.degrees);
        };

        const loadSubjects = async (degreesList) => {
            let subjectPromises = [];

            // Create promises for fetching subjects
            degreesList.forEach(degree => {
                subjectPromises.push(pull(`${BACKEND_URL}/api/StudyInfo/GetDegreeInfo?degreeName=${degree.name}`));
            });

            // Wait for all subject fetches to complete
            const subjectResponses = await Promise.all(subjectPromises);
            let allSubjects = [];

            // Flatten all the subject responses into one array
            subjectResponses.forEach(response => {
                allSubjects = [...allSubjects, ...response.subjects];
            });

            setSubjects(allSubjects); // Now set the subjects after all are fetched
            setLoading(false);
            console.log(allSubjects);
        }

        loadDegrees();
    }, []);

    function handleSubjectClick(subject) {
        //setSelectedSubject(subject); 
        //console.log('clicked', subject)
    }

    function renderSubjects(degree)
    {
        const degreeSubjects = subjects.filter(subject => subject.degree.name === degree.name);
        console.log(degreeSubjects);

        return (
            <>
                <div className='subjectGrid'>
                    {
                        degreeSubjects.map(subject => {
                            return (
                                <div key={subject.Id} className='subjectItem gridItem' onClick={() => handleSubjectClick(subject)}>
                                    <h3><strong>{subject.courseName}</strong></h3>
                                    <p><strong>Studiepunten: </strong>{subject.points}</p>
                                    <p><strong>Semester: </strong>{subject.semester}</p>
                                    <p><strong>Datum: </strong>{subject.date}</p>

                                </div>
                            )
                        })
                    }
                </div>
            </>
        )
    }

    function renderDegrees()
    {
        return (
            <>
            {degrees.length > 0 ? (
                <div className='degreeGridWrapper'>
                    <div className='degreeGrid'>
                        {
                            degrees.map(degree => {
                                return (
                                    <div key={degree.Id} className='degreeItem gridItem'>
                                    <h2>{degree.name}</h2>
                                    <div className="circleContainer">
                                        <ProgressionCircle percentage={calculateDegreePercentage(degree)} color="blue" />
                                    </div>
                                    <div className="subjectGrid">
                                        {renderSubjects(degree)}
                                    </div>
                                </div>
                                
                                )
                            })  
                        }
                    </div>
                </div>
            ) : (
                <p>No degrees were found</p>
                )
            } 

            {selectedSubject && (
                <div className="subjectCardWrapper">
                    <SubjectCard subject={selectedSubject} />
                </div>
            )}
            </>
        )
    }

    return <>
        <h1 className="centerh1">Studie Progressie</h1>

        {loading ? (
            <p>Loading...</p>
        ) : (
            renderDegrees()  
            )
        }
    </>
}

export default StudyProgression;