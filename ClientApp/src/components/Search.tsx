import React, { useState } from 'react';
import { Form, FormGroup, Label, Input, Button } from 'reactstrap';
import Axios from 'axios';
import swal from 'sweetalert';
import YouTube from "react-youtube";

class VideoResult {
  videoId: string;
  from: number;
  to: number;

  constructor() {
    this.videoId = '';
    this.from = 0;
    this.to = 0;
  }
}

export function Search() {
  const [searchQuery, setSearchQuery] = useState<string>('');
  const [videos, setVideos] = useState<VideoResult[]>(new Array<VideoResult>());
  const [currentVideoIndex, setCurrentVideoIndex] = useState<number>(-1);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    debugger;
    e.preventDefault();
    try {
      const res = await Axios.get<VideoResult[]>(`api/search/${searchQuery}`);
      setVideos(res.data);
      setCurrentVideoIndex(0);
    } catch (error) {
      swal({ text: 'Some error happened', timer: 5000 });
    }
  }

  const changed = function (event: { target: any; data: number; }) {
    if (event.data === YouTube.PlayerState.ENDED || event.data === YouTube.PlayerState.PAUSED) {
      if (currentVideoIndex + 1 < videos.length)
        setCurrentVideoIndex(currentVideoIndex + 1);
    }
  }

  const currentVideo = videos[currentVideoIndex];

  const renderedVideo = currentVideo ?
    <div>
      <YouTube
        onStateChange={changed}
        videoId={currentVideo.videoId}
        opts={{ width: "100%", playerVars: { autoplay: 1, start: currentVideo.from, end: currentVideo.to } }}>
      </YouTube>
    </div> : <></>;

  return (
    <>
      <Form>
        <FormGroup>
          <Label><h1>Search Query:</h1></Label>
          <div style={{ display: "flex" }}>
            <Input style={{ width: "90%" }} type="text" placeholder="Aufruf" value={searchQuery} onChange={(e) => setSearchQuery(e.target.value)} />
            <Button style={{ width: "10%" }} type="submit" color="primary" onClick={handleSubmit}>Save</Button>
          </div>
        </FormGroup>
      </Form>
      {renderedVideo}
    </>
  );
}
