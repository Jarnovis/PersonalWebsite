import React from "react";

function SubjectCard({subject}) {
    console.log('card', subject);
    return (
        <>
            <div className='subjectCard'>
                <h1><strong>{subject.courseName}</strong></h1>
                <p><strong>Studiepunten: </strong>{subject.points}</p>
                <p><strong>Semester: </strong>{subject.semester}</p>
                <p><strong>Datum: </strong>{subject.date}</p>
            </div>
        </>
    )
}

export default SubjectCard;