import React from "react";
import styles from "./styles.module.css";
import {MAX_HEIGHT, MAX_WIDTH} from "../../consts/sizes";

export default function Person({person, onClick, withInfectionRadius}) {
    const x = (person.position.x / MAX_WIDTH) * 100;
    const y = (person.position.y / MAX_HEIGHT) * 100;

    const isDoctor = person.personType === "Doctor";
    const isIll = person.healthStatus === 'Ill';
    
    let req = styles.healthy;
    if (person.isBored) {
        req = styles.bored
    }

    if (isIll) req = styles.ill;
  
    return (
      <div className={styles.root} style={{left: `${x}%`, top: `${y}%`}} onClick={() => onClick(person.id)} >
        <div className={withInfectionRadius && isIll ? styles.infectionRadius : ""} />
        <div className={`${styles.person} ${styles[person.healthStatus.toLowerCase()]} ${req}  ${isDoctor ? styles.doctor : null}`}
        onClick={() => onClick(person.id)} />
      </div>
    );
}

